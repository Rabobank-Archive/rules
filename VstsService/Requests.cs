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
    }
}