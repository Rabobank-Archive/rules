using System;
using RestSharp;
using Environment = SecurePipelineScan.VstsService.Response.Environment;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Release
    {
        public static IVstsRestRequest<Response.Release> Releases(string project, string id)
        {
            return new VsrmRequest<Response.Release>($"{project}/_apis/release/releases/{id}");
        }

        public static IVstsRestRequest<Response.ReleaseDefinition> Definition(string project, string id)
        {
            return new VsrmRequest<Response.ReleaseDefinition>($"{project}/_apis/release/definitions/{id}");
        }

        public static IVstsRestRequest<Response.Multiple<Response.ReleaseDefinition>> Definitions(string project)
        {
            return new VsrmRequest<Response.Multiple<Response.ReleaseDefinition>>($"{project}/_apis/release/definitions/");
        }

        public static IVstsRestRequest<Response.Environment> Environment(string project, string release, string id)
        {
            return new VsrmRequest<Environment>($"{project}/_apis/release/releases/{release}/environments/{id}");
        }
        
        public static IVstsRestRequest<Response.Multiple<Response.Tag>> Tags(string project, string release)
        {
            return new VsrmRequest<Response.Multiple<Response.Tag>>($"{project}/_apis/Release/releases/{release}/tags");
        }
    }
}