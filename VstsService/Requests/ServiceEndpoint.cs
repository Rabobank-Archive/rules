
namespace SecurePipelineScan.VstsService.Requests
{
    public static class ServiceEndpoint
    {
        public static IEnumerableRequest<Response.ServiceEndpointHistory> History(string project, string id) => 
            new VstsRequest<Response.ServiceEndpointHistory>($"{project}/_apis/serviceendpoint/{id}/executionhistory").AsEnumerable();

        public static IEnumerableRequest<Response.ServiceEndpoint> Endpoints(string project) => 
            new VstsRequest<Response.ServiceEndpoint>($"{project}/_apis/serviceendpoint/endpoints/").AsEnumerable();
    }
}