using SecurePipelineScan.VstsService;
using SecurePipelineScan.Rules.Release;
using System;
using RestSharp;
using System.Linq;

namespace SecurePipelineScan.Rules
{
    public static class Scan
    {
        public static void Execute(IVstsRestClient client, string project)
        {
            var endpoints = client.Execute(Requests.ServiceEndpoint.Endpoints(project));
            endpoints.Data.Value = endpoints.Data.Value.Where(ep => ep.Name == "rg-m01-prd-vsts-01-deploy (SPN)");
            foreach (var endpoint in endpoints.Data.Value)
            {
                Console.WriteLine(endpoint.Name);
                foreach (var history in client.Execute(Requests.ServiceEndpoint.History(project, endpoint.Id)).Data.Value.GroupBy(h => h.Data.Definition.Name))
                {
                    Console.WriteLine($"  {history.Key}");
                    foreach (var item in history)
                    {
                        var release = client.Execute(new VstsRestRequest<SecurePipelineScan.VstsService.Response.Release>(item.Data.Owner.Links.Self.Href.AbsoluteUri, Method.GET));
                        if (!string.IsNullOrEmpty(release.ErrorMessage))
                        {
                            throw new Exception(release.ErrorMessage);
                        };

                        var rule = new FourEyesOnAllBuildArtefacts();
                        Console.WriteLine($"    {release.Data.Id} {item.Data.Owner.Name}: {ColorCode(rule.GetResult(release.Data, item.Data.Owner.Id ))}");
                    }
                }
            }
        }

        private static string ColorCode(bool result)
        {
            return result ? "\u001b[32mV\u001b[0m" : "\u001b[31mX\u001b[0m";
        }
    }
}