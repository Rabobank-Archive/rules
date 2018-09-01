using RestSharp;

namespace lib.tests.requests
{
    internal class ReleaseDefinition : RestRequest
    {
        public string Project { get; }
        public string Id { get; }

        public ReleaseDefinition(string project, string id) : base("{project}/_apis/release/definitions/{id}", Method.GET)
        {
             base
                .AddUrlSegment("project", project)
                .AddUrlSegment("id", id);

            Project = project;
            Id = id;
        }
    }
}
