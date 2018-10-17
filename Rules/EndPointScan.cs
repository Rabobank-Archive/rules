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
    public class EndPointScan
    {
        private readonly IVstsRestClient client;

        public EndPointScan(IVstsRestClient client)
        {
            this.client = client;
        }

        public IEnumerable<EndpointReport> Execute(string project)
        {
            var endpoints = client.Execute(Requests.ServiceEndpoint.Endpoints(project));
            return Execute(project, endpoints.ThrowOnError().Data.Value);
        }

        private IEnumerable<EndpointReport> Execute(string project, IEnumerable<Response.ServiceEndpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                foreach (var report in Execute(project, endpoint))
                {
                    yield return report;
                }
            }
        }

        private IEnumerable<EndpointReport> Execute(string project, Response.ServiceEndpoint endpoint)
        {
            foreach (var history in client
                .Execute(Requests.ServiceEndpoint.History(project, endpoint.Id))
                .ThrowOnError()
                .Data.Value.OrderBy(h => h.Data.Definition.Name))
            {
                yield return Execute(endpoint, history);
            }
        }

        private EndpointReport Execute(Response.ServiceEndpoint endpoint, Response.ServiceEndpointHistory history)
        {
            switch (history.Data.PlanType)
            {
                case "Release":
                    return PrintRelease(endpoint, history);

                default:
                    return PrintOther(endpoint, history);
            }
        }

        private EndpointReport PrintOther(Response.ServiceEndpoint endpoint, Response.ServiceEndpointHistory item)
        {
            return new Unknown
            {
                Endpoint = endpoint,
                Request = item.Data
            };
        }

        private EndpointReport PrintRelease(Response.ServiceEndpoint endpoint, Response.ServiceEndpointHistory item)
        {
            var release = client
                .Execute(new VstsRestRequest<VstsService.Response.Release>(item.Data.Owner.Links.Self.Href.AbsoluteUri, Method.GET))
                .ThrowOnError();

            var rule = new FourEyesOnAllBuildArtefacts();
            return new ReleaseReport
            {
                Release = release.Data,
                Endpoint = endpoint,
                Request = item.Data,
                Result = rule.GetResult(release.Data, item.Data.Owner.Id)
            };
        }
    }
}