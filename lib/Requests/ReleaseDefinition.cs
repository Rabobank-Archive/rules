using RestSharp;

namespace lib.Requests
{
    public class ReleaseDefinition : RestRequest, IVsrmRequest
    {
        public string Project { get; }
        public string Id { get; }

        public ReleaseDefinition(string project, string id) : base($"{project}/_apis/release/definitions/{id}", Method.GET)
        {
            Project = project;
            Id = id;
        }
    }
}
