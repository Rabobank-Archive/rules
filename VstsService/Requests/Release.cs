using System.Collections.Generic;
using Environment = SecurePipelineScan.VstsService.Response.Environment;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ReleaseManagement
    {
        public static IVstsRequest<Response.Release> Release(string project, string id) => 
            new VsrmRequest<Response.Release>($"{project}/_apis/release/releases/{id}");

        public static IEnumerableRequest<Response.Release> Releases(string project) => 
            new VsrmRequest<Response.Release>($"{project}/_apis/release/releases").AsEnumerable();

        public static IEnumerableRequest<Response.Release> Releases(string project, string expand, string asof) =>
            new VsrmRequest<Response.Release>($"{project}/_apis/release/releases", new Dictionary<string, object> 
            {
                { "expand", $"{expand}" },
                { "minCreatedTime", $"{asof}" }
            }).AsEnumerable();

        public static IEnumerableRequest<Response.Release> Releases(string project, string pipelineId, 
                IEnumerable<string> stageIds, string expand, string asof, string ct) =>
            new VsrmRequest<Response.Release>($"{project}/_apis/release/releases", 
                new Dictionary<string, object>
                {
                    { "definitionId", $"{pipelineId}" },
                    { "definitionEnvironmentId", $"{string.Join("|", stageIds)}" },
                    { "$expand", $"{expand}" },
                    { "minCreatedTime", $"{asof}" },
                    { "continuationToken", $"{ct}" }
                }).AsEnumerable();

        public static IEnumerableRequest<Response.Release> Releases(string project, string pipelineId,
                IEnumerable<string> stageIds, string expand, string asof) =>
            new VsrmRequest<Response.Release>($"{project}/_apis/release/releases",
                new Dictionary<string, object>
                {
                    { "definitionId", $"{pipelineId}" },
                    { "definitionEnvironmentId", $"{string.Join("|", stageIds)}" },
                    { "$expand", $"{expand}" },
                    { "minCreatedTime", $"{asof}" },
                }).AsEnumerable();

        public static IVstsRequest<Response.ReleaseDefinition> Definition(string project, string id) => 
            new VsrmRequest<Response.ReleaseDefinition>($"{project}/_apis/release/definitions/{id}");

        public static IEnumerableRequest<Response.ReleaseDefinition> Definitions(string project) => 
            new VsrmRequest<Response.ReleaseDefinition>($"{project}/_apis/release/definitions/").AsEnumerable();

        public static IVstsRequest<Environment> Environment(string project, string release, string id) => 
            new VsrmRequest<Environment>($"{project}/_apis/release/releases/{release}/environments/{id}");

        public static IVstsRequest<Response.ReleaseSettings> Settings(string project) =>
            new VsrmRequest<Response.ReleaseSettings>($"{project}/_apis/release/releasesettings", new Dictionary<string, object> 
            {
                { "api-version", "5.0-preview" }
            });
    }
}