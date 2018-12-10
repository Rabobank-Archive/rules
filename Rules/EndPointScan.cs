using RestSharp;
using SecurePipelineScan.Rules.Release;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService.Requests;
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
            var endpoints = client.Get(ServiceEndpoint.Endpoints(project));
            return Execute(project, endpoints.Value);
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
                .Get(ServiceEndpoint.History(project, endpoint.Id)).Value.OrderBy(h => h.Data.Definition.Name))
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
                .Get(new VstsRestRequest<VstsService.Response.Release>(item.Data.Owner.Links.Self.Href.AbsoluteUri));

            var rule = new IsStageApproved();
            return new ReleaseReport
            {
                Release = release,
                Endpoint = endpoint,
                Request = item.Data,
                Result = rule.GetResult(release, item.Data.Owner.Id)
            };
        }
    }
}