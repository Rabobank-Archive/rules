using SecurePipelineScan.VstsService;
using SecurePipelineScan.Rules.Release;
using System;
using RestSharp;
using System.Linq;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules
{
    public class Scan
    {
        private readonly IVstsRestClient client;
        private readonly Action<string> output;

        public Scan(IVstsRestClient client, Action<string> output)
        {
            this.client = client;
            this.output = output;
        }
        public void Execute(string project)
        {
            var endpoints = client.Execute(Requests.ServiceEndpoint.Endpoints(project));
            foreach (var endpoint in endpoints.Data.Value)
            {
                output(endpoint.Name);
                foreach (var history in client.Execute(Requests.ServiceEndpoint.History(project, endpoint.Id)).Data.Value.GroupBy(h => h.Data.Definition.Name))
                {
                    output($"  {history.Key}");
                    foreach (var item in history)
                    {
                        switch (item.Data.PlanType)
                        {
                            case "Release":
                                PrintRelease(client, item);
                                break;
                            default:
                                PrintOther(client, item);
                                break;
                        }
                    }
                }
            }
        }

        private void PrintOther(IVstsRestClient client, ServiceEndpointHistory item)
        {
            output($"    {item.Data.Definition.Name} {item.Data.Owner.Name}: \u001b[93m?\u001b[0m");
        }

        private void PrintRelease(IVstsRestClient client, VstsService.Response.ServiceEndpointHistory item)
        {
            var release = client.Execute(new VstsRestRequest<VstsService.Response.Release>(item.Data.Owner.Links.Self.Href.AbsoluteUri, Method.GET));
            if (!string.IsNullOrEmpty(release.ErrorMessage))
            {
                throw new Exception(release.ErrorMessage);
            };

            var rule = new FourEyesOnAllBuildArtefacts();
            output($"    {release.Data.Id} {item.Data.Owner.Name}: {ColorCode(rule.GetResult(release.Data, item.Data.Owner.Id))}");
        }

        private static string ColorCode(bool result)
        {
            return result ? "\u001b[32mV\u001b[0m" : "\u001b[31mX\u001b[0m";
        }
    }
}