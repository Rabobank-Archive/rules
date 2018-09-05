using RestSharp;

namespace vsts
{
    public static class Requests
    {
        public static class Release
        {
            public static IVstsRestRequest<vsts.Response.Release> Releases(string project, string id)
            {
                return new VsrmRequest<vsts.Response.Release>($"{project}/_apis/release/releases/{id}", Method.GET);
            }

            public static IVstsRestRequest<vsts.Response.ReleaseDefinition> Definition(string project, string id)
            {
                return new VsrmRequest<vsts.Response.ReleaseDefinition>($"{project}/_apis/release/definitions/{id}", Method.GET);
            }

            public static IVstsRestRequest<vsts.Response.Multiple<vsts.Response.ReleaseDefinition>> Definitions(string project) 
            {
                return new VsrmRequest<vsts.Response.Multiple<vsts.Response.ReleaseDefinition>>($"{project}/_apis/release/definitions/", Method.GET);
            }
        }

        public static class ServiceEndpoint
        {
            public static IVstsRestRequest<vsts.Response.Multiple<vsts.Response.ServiceEndpointHistory>> History(string project, string id)
            {
                return new VstsRestRequest<vsts.Response.Multiple<vsts.Response.ServiceEndpointHistory>>($"{project}/_apis/serviceendpoint/{id}/executionhistory", Method.GET);
            }

            public static IVstsRestRequest<vsts.Response.Multiple<vsts.Response.ServiceEndpoint>> Endpoints(string project)
            {
                return new VstsRestRequest<vsts.Response.Multiple<vsts.Response.ServiceEndpoint>>($"{project}/_apis/serviceendpoint/endpoints/", Method.GET);
            }
        }
    }
}