
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Repository
    {
        public static IEnumerableRequest<Response.Repository> Repositories(string project) => 
            new VstsRequest<Response.Repository>($"{project}/_apis/git/repositories").AsEnumerable();

        public static IEnumerableRequest<Push> Pushes(string project, string repositoryId) => 
            new VstsRequest<Push>($"/{project}/_apis/git/repositories/{repositoryId}/pushes").AsEnumerable();
    }
}