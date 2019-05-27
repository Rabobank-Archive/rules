using System;
using RestSharp;
using Environment = SecurePipelineScan.VstsService.Response.Environment;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Release
    {
        public static IVstsRequest<Response.Release> Releases(string project, string id)
        {
            return new VsrmRequest<Response.Release>($"{project}/_apis/release/releases/{id}");
        }

        public static IVstsRequest<Response.ReleaseDefinition> Definition(string project, string id)
        {
            return new VsrmRequest<Response.ReleaseDefinition>($"{project}/_apis/release/definitions/{id}");
        }

        public static IVstsRequest<Response.Multiple<Response.ReleaseDefinition>> Definitions(string project)
        {
            return new VsrmRequest<Response.Multiple<Response.ReleaseDefinition>>($"{project}/_apis/release/definitions/");
        }

        public static IVstsRequest<Response.Environment> Environment(string project, string release, string id)
        {
            return new VsrmRequest<Environment>($"{project}/_apis/release/releases/{release}/environments/{id}");
        }
       
    }
}