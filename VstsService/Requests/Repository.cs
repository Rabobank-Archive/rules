
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Repository
    {
        public static IEnumerableRequest<Response.Repository> Repositories(string project) => 
            new VstsRequest<Response.Repository>($"{project}/_apis/git/repositories").AsEnumerable();

        public static IVstsRequest<Response.Repository> Repo(string project, string repositoryId) =>
            new VstsRequest<Response.Repository>($"/{project}/_apis/git/repositories/{repositoryId}");

        public static IEnumerableRequest<Push> Pushes(string project, string repositoryId) => 
            new VstsRequest<Push>($"/{project}/_apis/git/repositories/{repositoryId}/pushes").AsEnumerable();

        public static IVstsRequest<JObject> GitItem(string project, string repositoryId, string yamlFilename) =>
            new VstsRequest<JObject>($"/{project}/_apis/git/repositories/{repositoryId}/items",
                new Dictionary<string, object>
                {
                    { "path", $"{yamlFilename}" },
                    { "includeContent", true },
                    { "$format", "json" }
                });
    }
}