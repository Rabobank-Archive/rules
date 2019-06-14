
namespace SecurePipelineScan.VstsService.Requests
{
    public static class ServiceEndpoint
    {
        public static IVstsRequest<Response.Multiple<Response.ServiceEndpointHistory>> History(string project, string id)
        {
            return new VstsRequest<Response.Multiple<Response.ServiceEndpointHistory>>($"{project}/_apis/serviceendpoint/{id}/executionhistory");
        }

        public static IVstsRequest<Response.Multiple<Response.ServiceEndpoint>> Endpoints(string project)
        {
            return new VstsRequest<Response.Multiple<Response.ServiceEndpoint>>($"{project}/_apis/serviceendpoint/endpoints/");
        }
    }
}