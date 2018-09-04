using RestSharp;

namespace lib
{
    public static class Requests
    {
        public static class Release
        {
            public static IVstsRequest<Response.Release> Releases(string project, string id)
            {
                return new VsrmRequest<Response.Release>($"{project}/_apis/release/releases/{id}", Method.GET);
            }

            public static IVstsRequest<Response.ReleaseDefinition> Definition(string project, string id)
            {
                return new VsrmRequest<Response.ReleaseDefinition>($"{project}/_apis/release/definitions/{id}", Method.GET);
            }

            public static IVstsRequest<Response.Multiple<Response.ReleaseDefinition>> Definitions(string project) 
            {
                return new VsrmRequest<Response.Multiple<Response.ReleaseDefinition>>($"{project}/_apis/release/definitions/", Method.GET);
            }
        }

        public static class ServiceEndpoint
        {
            public static IVstsRequest<Response.Multiple<Response.ServiceEndpointHistory>> History(string project, string id)
            {
                return new VstsRequest<Response.Multiple<Response.ServiceEndpointHistory>>($"{project}/_apis/serviceendpoint/{id}/executionhistory", Method.GET);
            }

            public static IVstsRequest<Response.Multiple<Response.ServiceEndpoint>> Endpoints(string project)
            {
                return new VstsRequest<Response.Multiple<Response.ServiceEndpoint>>($"{project}/_apis/serviceendpoint/endpoints/", Method.GET);
            }
        }
    }
}