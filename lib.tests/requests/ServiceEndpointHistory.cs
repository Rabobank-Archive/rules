using RestSharp;

namespace lib.tests.Requests
{
    internal class ServiceEndpointHistory : RestRequest
    {
        public string Project { get; }

        public ServiceEndpointHistory(string project, string id) : base("{project}/_apis/serviceendpoint/{id}/executionhistory", Method.GET)
        {
            base
                .AddUrlSegment("project", project)
                .AddUrlSegment("id", id);

            this.Project = project;
        }
    }
}