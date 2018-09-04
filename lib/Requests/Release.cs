using RestSharp;

namespace lib.Requests
{
    public class Release : RestRequest, IVsrmRequest<Response.Release>
    {
        public Release(string project, string id) : base($"{project}/_apis/release/releases/{id}")
        {
        }
    }
}