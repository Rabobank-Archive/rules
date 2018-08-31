using RestSharp;

namespace lib.tests
{
    static class Requests
    {
        public static IRestRequest ReleaseDefinition(string project, string id)
        {
            return new RestRequest(id, Method.GET)
                .AddUrlSegment("project", project)
                .AddUrlSegment("area", "release")
                .AddUrlSegment("resource", "definitions");
        }

        public static IRestRequest ServiceEndpoints(string project)
        {
            return new RestRequest(Method.GET)
                .AddUrlSegment("project", project)
                .AddUrlSegment("area", "serviceendpoint")
                .AddUrlSegment("resource", "endpoints");
        }

        public static IRestRequest ReleaseDefinitions(string project)
        {
            return new RestRequest(Method.GET)
                .AddUrlSegment("project", project)
                .AddUrlSegment("area", "release")
                .AddUrlSegment("resource", "definitions");
        }
    }
}

