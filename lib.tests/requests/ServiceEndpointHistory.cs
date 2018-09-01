using RestSharp;

namespace lib.tests.Requests
{
    internal class ServiceEndpointHistory : RestRequest
    {
        public string Project { get; }
        public string Id { get; }

        public ServiceEndpointHistory(string project, string id) : base($"{project}/_apis/serviceendpoint/{id}/executionhistory", Method.GET)
        {
            Project = project;
            Id = id;
        }
    }
}