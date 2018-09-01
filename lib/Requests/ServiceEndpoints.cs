using RestSharp;

namespace lib.Requests
{
    public class ServiceEndpoints : RestRequest
    {
        public string Project { get; }

        public ServiceEndpoints(string project) : base($"{project}/_apis/serviceendpoint/endpoints/", Method.GET)
        {
            Project = project;
        }
    }
}