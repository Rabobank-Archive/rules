using RestSharp;

namespace lib.tests.requests
{
    internal class ReleaseDefinition : RestRequest
    {
        public string Project { get; }
        public string Id { get; }

        public ReleaseDefinition(string project, string id) : base(id, Method.GET)
        {
             base
                .AddUrlSegment("project", project)
                .AddUrlSegment("area", "release")
                .AddUrlSegment("resource", "definitions");

            Project = project;
            Id = id;
        }
    }
}
