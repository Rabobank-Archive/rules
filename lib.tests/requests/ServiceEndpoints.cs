using RestSharp;

namespace lib.tests.requests
{
    internal class ServiceEndpoints : RestRequest
    {
        public string Project { get; }

        public ServiceEndpoints(string project) : base("{project}/_apis/serviceendpoint/endpoints/", Method.GET)
        {
            base
                .AddUrlSegment("project", project);

            this.Project = project;
        }
    }
}