using RestSharp;

namespace lib.tests.requests
{
    internal class ServiceEndpoints : RestRequest
    {
        public string Project { get; }

        public ServiceEndpoints(string project) : base(Method.GET)
        {
            base
                .AddUrlSegment("project", project)
                .AddUrlSegment("area", "serviceendpoint")
                .AddUrlSegment("resource", "endpoints");
                
            this.Project = project;
        }
    }
}