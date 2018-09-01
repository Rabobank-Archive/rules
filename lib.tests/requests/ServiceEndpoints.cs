using RestSharp;

namespace lib.tests.Requests
{
    internal class ServiceEndpoints : RestRequest
    {
        public string Project { get; }

        public ServiceEndpoints(string project) : base($"{project}/_apis/serviceendpoint/endpoints/", Method.GET)
        {
            Project = project;
        }
    }
}