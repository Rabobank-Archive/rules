using System;
using RestSharp;

namespace SecurePipelineScan.VstsService
{
    public static class Requests
    {
        public static class Release
        {
            public static IVstsRestRequest<Response.Release> Releases(string project, string id)
            {
                return new VsrmRequest<Response.Release>($"{project}/_apis/release/releases/{id}", Method.GET);
            }

            public static IVstsRestRequest<Response.ReleaseDefinition> Definition(string project, string id)
            {
                return new VsrmRequest<Response.ReleaseDefinition>($"{project}/_apis/release/definitions/{id}", Method.GET);
            }

            public static IVstsRestRequest<Response.Multiple<Response.ReleaseDefinition>> Definitions(string project) 
            {
                return new VsrmRequest<Response.Multiple<Response.ReleaseDefinition>>($"{project}/_apis/release/definitions/", Method.GET);
            }
        }

        public static IVstsRestRequest<Response.Multiple<Response.Project>> Projects()
        {
            return new VstsRestRequest<Response.Multiple<Response.Project>>($"_apis/projects?$top=1000&api-version=4.1-preview.2",Method.GET);
        }

        public static class ServiceEndpoint
        {
            public static IVstsRestRequest<Response.Multiple<Response.ServiceEndpointHistory>> History(string project, string id)
            {
                return new VstsRestRequest<Response.Multiple<Response.ServiceEndpointHistory>>($"{project}/_apis/serviceendpoint/{id}/executionhistory", Method.GET);
            }

            public static IVstsRestRequest<Response.Multiple<Response.ServiceEndpoint>> Endpoints(string project)
            {
                return new VstsRestRequest<Response.Multiple<Response.ServiceEndpoint>>($"{project}/_apis/serviceendpoint/endpoints/", Method.GET);
            }
        }

        public static class Repository
        {
            public static IVstsRestRequest<Response.Multiple<Response.Repository>> Repositories(string project)
            {
                return new VstsRestRequest<Response.Multiple<Response.Repository>>($"{project}/_apis/git/repositories", Method.GET);
            }
        }

        public static class Policies
        {
            public static IVstsRestRequest<Response.Multiple<Response.RequiredReviewersPolicy>> RequiredReviewersPolicies(string project)
            {
                return new VstsRestRequest<Response.Multiple<Response.RequiredReviewersPolicy>>($"{project}/_apis/policy/configurations?policyType=fd2167ab-b0be-447a-8ec8-39368250530e", Method.GET);
            }

            public static IVstsRestRequest<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>> MinimumNumberOfReviewersPolicies(string project)
            {
                return new VstsRestRequest<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>($"{project}/_apis/policy/configurations?policyType=fa4e907d-c16b-4a4c-9dfa-4906e5d171dd", Method.GET);
            }
        }

        public static class DistributedTask
        {
            public static IVstsRestRequest<Response.Multiple<Response.AgentPoolInfo>> OrganizationalAgentPools()
            {
                return new VstsRestRequest<Response.Multiple<Response.AgentPoolInfo>>($"_apis/distributedtask/pools",Method.GET);
            }

            public static IVstsRestRequest<Response.AgentPoolInfo> AgentPool(int id)
            {
                return new VstsRestRequest<Response.AgentPoolInfo>($"_apis/distributedtask/pools/{id}", Method.GET);
            }

            public static IVstsRestRequest<Response.Multiple<Response.AgentStatus>> AgentPoolStatus(int id)
            {
                return new VstsRestRequest<Response.Multiple<Response.AgentStatus>>($"_apis/distributedtask/pools/{id}/agents?includeCapabilities=false&includeAssignedRequest=true", Method.GET);
            }

        }
    }
}