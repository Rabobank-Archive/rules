using RestSharp;

namespace lib.Requests
{
    public class Release : RestRequest, IVsrmRequest
    {
        public string Project { get; }
        public string Id { get; }

        public Release(string project, string id) : base($"{project}/_apis/release/releases/{id}")
        {
            Project = project;
            Id = id;
        }
    }
}