using RestSharp;

namespace lib.Requests
{
    public class ReleaseDefinitions : RestRequest, IVsrmRequest
    {
        public string Project { get; }

        public ReleaseDefinitions(string project) : base($"{project}/_apis/release/definitions/", Method.GET)
        {
            this.Project = project;
        }
    }

}

