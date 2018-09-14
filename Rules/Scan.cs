using SecurePipelineScan.VstsService;
using SecurePipelineScan.Rules.Release;
using System;
using RestSharp;
using System.Linq;
using Response = SecurePipelineScan.VstsService.Response;
using SecurePipelineScan.Rules.Reports;

namespace SecurePipelineScan.Rules
{
    public class Scan
    {
        private readonly IVstsRestClient client;
        private readonly Action<ScanReport> progress;

        public Scan(IVstsRestClient client, Action<ScanReport> progress)
        {
            this.client = client;
            this.progress = progress;
        }
        public void Execute(string project)
        {
            var endpoints = client.Execute(Requests.ServiceEndpoint.Endpoints(project));
            foreach (var endpoint in endpoints.ThrowOnError().Data.Value)
            {
                // progress(endpoint.Name);
                foreach (var history in client
                    .Execute(Requests.ServiceEndpoint.History(project, endpoint.Id))
                    .ThrowOnError()
                    .Data.Value.GroupBy(h => h.Data.Definition.Name))
                {
                    // progress($"  {history.Key}");
                    foreach (var item in history)
                    {
                        switch (item.Data.PlanType)
                        {
                            case "Release":
                                PrintRelease(client, endpoint, item);
                                break;
                            default:
                                PrintOther(endpoint, item);
                                break;
                        }
                    }
                }
            }
        }

        private void PrintOther(Response.ServiceEndpoint endpoint, Response.ServiceEndpointHistory item)
        {
            progress(new Unknown
            {
                Endpoint = endpoint,
                Request = item.Data
            });
        }

        private void PrintRelease(IVstsRestClient client, Response.ServiceEndpoint endpoint, Response.ServiceEndpointHistory item)
        {
            var release = client
                .Execute(new VstsRestRequest<VstsService.Response.Release>(item.Data.Owner.Links.Self.Href.AbsoluteUri, Method.GET))
                .ThrowOnError();

            var rule = new FourEyesOnAllBuildArtefacts();
            progress(new ReleaseReport
            {
                Release = release.Data,
                Endpoint = endpoint,
                Request = item.Data,
                Result = rule.GetResult(release.Data, item.Data.Owner.Id)
            });
        }
    }
}