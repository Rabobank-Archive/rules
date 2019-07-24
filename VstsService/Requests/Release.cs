using System.Collections.Generic;
using Environment = SecurePipelineScan.VstsService.Response.Environment;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ReleaseManagement
    {
        public static IVstsRequest<Response.Release> Release(string project, string id)
        {
            return new VsrmRequest<Response.Release>($"{project}/_apis/release/releases/{id}");
        }

        public static IVstsRequest<Response.Multiple<Response.Release>> Releases(string project)
        {
            return new VsrmRequest<Response.Multiple<Response.Release>>($"{project}/_apis/release/releases");
        }

        public static IVstsRequest<Response.Multiple<Response.Release>> Releases(string project, string expand, string asof)
        {
            return new VsrmRequest<Response.Multiple<Response.Release>>($"{project}/_apis/release/releases", new Dictionary<string, object> 
            {
                { "expand", $"{expand}" },
                { "minCreatedTime", $"{asof}" }
            });
        }

        public static IVstsRequest<Response.ReleaseDefinition> Definition(string project, string id)
        {
            return new VsrmRequest<Response.ReleaseDefinition>($"{project}/_apis/release/definitions/{id}");
        }

        public static IVstsRequest<Response.Multiple<Response.ReleaseDefinition>> Definitions(string project)
        {
            return new VsrmRequest<Response.Multiple<Response.ReleaseDefinition>>($"{project}/_apis/release/definitions/");
        }

        public static IVstsRequest<Environment> Environment(string project, string release, string id)
        {
            return new VsrmRequest<Environment>($"{project}/_apis/release/releases/{release}/environments/{id}");
        }

        public static IVstsRequest<Response.ReleaseSettings> Settings(string project)
        {
            return new VsrmRequest<Response.ReleaseSettings>($"{project}/_apis/release/releasesettings", new Dictionary<string, object> 
            {
                { "api-version", "5.0-preview" }
            });
        }
    }
}