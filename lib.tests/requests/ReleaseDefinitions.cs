using RestSharp;

namespace lib.tests.Requests
{
    internal class ReleaseDefinitions : RestRequest
    {
        public string Project { get; }

        public ReleaseDefinitions(string project) : base("{project}/_apis/release/definitions/", Method.GET)
        {
            base
                .AddUrlSegment("project", project);
            
            this.Project = project;
        }
    }

}

