using RestSharp;

namespace lib.Requests
{
    public class ReleaseDefinitions : RestRequest, IVsrmRequest<Response.Multiple<Response.ReleaseDefinition>>
    {

        public ReleaseDefinitions(string project) : base($"{project}/_apis/release/definitions/", Method.GET)
        {
        }
    }

}

