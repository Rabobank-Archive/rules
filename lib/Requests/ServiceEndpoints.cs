using RestSharp;

namespace lib.Requests
{
    public class ServiceEndpoints : RestRequest, IVstsRequest<Response.Multiple<Response.ServiceEndpoint>>
    {
        public ServiceEndpoints(string project) : base($"{project}/_apis/serviceendpoint/endpoints/", Method.GET)
        {
        }
    }
}