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
    }
}