
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

        public static IEnumerableRequest<GitRef> Refs(string project, string repositoryId) =>
            new VstsRequest<GitRef>($"/{project}/_apis/git/repositories/{repositoryId}/refs").AsEnumerable();

        public static IEnumerableRequest<Push> Pushes(string project, string repositoryId) => 
            new VstsRequest<Push>($"/{project}/_apis/git/repositories/{repositoryId}/pushes").AsEnumerable();

        public static IVstsRequest<JObject> GitItem(string project, string repositoryId, string path) =>
            new VstsRequest<JObject>($"/{project}/_apis/git/repositories/{repositoryId}/items",
                new Dictionary<string, object>
                {
                    { "path", $"{path}" },
                    { "includeContent", true },
                    { "$format", "json" },
                    { "versionDescriptor.versionType", "branch" },
                    { "versionDescriptor.version", "master" }
                });
    }
}