using RestSharp;

namespace lib.Requests
{
    public class ReleaseDefinition : RestRequest, IVsrmRequest<Response.ReleaseDefinition>
    {
        public ReleaseDefinition(string project, string id) : base($"{project}/_apis/release/definitions/{id}", Method.GET)
        {
        }
    }
}
