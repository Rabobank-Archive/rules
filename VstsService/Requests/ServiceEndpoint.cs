using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
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