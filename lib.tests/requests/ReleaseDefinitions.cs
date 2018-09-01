using RestSharp;

namespace lib.tests.requests
{
    internal class ReleaseDefinitions : RestRequest
    {
        public string Project { get; }

        public ReleaseDefinitions(string project) : base(Method.GET)
        {
            base
                .AddUrlSegment("project", project)
                .AddUrlSegment("area", "release")
                .AddUrlSegment("resource", "definitions");
            
            this.Project = project;
        }
    }

}

