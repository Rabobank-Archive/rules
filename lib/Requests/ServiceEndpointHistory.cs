using RestSharp;

namespace lib.Requests
{
    public class ServiceEndpointHistory : RestRequest, IVstsRequest<Response.Multiple<Response.ServiceEndpointHistory>>
    {
        public ServiceEndpointHistory(string project, string id) : base($"{project}/_apis/serviceendpoint/{id}/executionhistory", Method.GET)
        {
        }
    }
}