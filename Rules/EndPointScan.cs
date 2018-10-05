using RestSharp;
using SecurePipelineScan.Rules.Release;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using System.Linq;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules
{
    public class EndPointScan : IScan
    {
        private readonly IVstsRestClient client;
        private readonly Action<ScanReport> progress;

        public EndPointScan(IVstsRestClient client, Action<ScanReport> progress)
        {
            this.client = client;
            this.progress = progress;
        }

        public void Execute(string project)
        {
            var endpoints = client.Execute(Requests.ServiceEndpoint.Endpoints(project));
            Execute(project, endpoints.ThrowOnError().Data.Value);
        }

        private void Execute(string project, IEnumerable<Response.ServiceEndpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                Execute(project, endpoint);
            }
        }

        private void Execute(string project, Response.ServiceEndpoint endpoint)
        {
            foreach (var history in client
                .Execute(Requests.ServiceEndpoint.History(project, endpoint.Id))
                .ThrowOnError()
                .Data.Value.OrderBy(h => h.Data.Definition.Name))
            {
                Execute(endpoint, history);
            }
        }

        private void Execute(Response.ServiceEndpoint endpoint, Response.ServiceEndpointHistory history)
        {
            switch (history.Data.PlanType)
            {
                case "Release":
                    PrintRelease(client, endpoint, history);
                    break;

                default:
                    PrintOther(endpoint, history);
                    break;
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