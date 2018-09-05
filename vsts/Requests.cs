using RestSharp;

namespace Vsts
{
    public static class Requests
    {
        public static class Release
        {
            public static IVstsRestRequest<Vsts.Response.Release> Releases(string project, string id)
            {
                return new VsrmRequest<Vsts.Response.Release>($"{project}/_apis/release/releases/{id}", Method.GET);
            }

            public static IVstsRestRequest<Vsts.Response.ReleaseDefinition> Definition(string project, string id)
            {
                return new VsrmRequest<Vsts.Response.ReleaseDefinition>($"{project}/_apis/release/definitions/{id}", Method.GET);
            }

            public static IVstsRestRequest<Vsts.Response.Multiple<Vsts.Response.ReleaseDefinition>> Definitions(string project) 
            {
                return new VsrmRequest<Vsts.Response.Multiple<Vsts.Response.ReleaseDefinition>>($"{project}/_apis/release/definitions/", Method.GET);
            }
        }

        public static class ServiceEndpoint
        {
            public static IVstsRestRequest<Vsts.Response.Multiple<Vsts.Response.ServiceEndpointHistory>> History(string project, string id)
            {
                return new VstsRestRequest<Vsts.Response.Multiple<Vsts.Response.ServiceEndpointHistory>>($"{project}/_apis/serviceendpoint/{id}/executionhistory", Method.GET);
            }

            public static IVstsRestRequest<Vsts.Response.Multiple<Vsts.Response.ServiceEndpoint>> Endpoints(string project)
            {
                return new VstsRestRequest<Vsts.Response.Multiple<Vsts.Response.ServiceEndpoint>>($"{project}/_apis/serviceendpoint/endpoints/", Method.GET);
            }
        }
    }
}