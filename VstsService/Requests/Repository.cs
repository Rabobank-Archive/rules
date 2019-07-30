
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Repository
    {
        public static IVstsRequest<Multiple<Response.Repository>> Repositories(string project)
        {
            return new VstsRequest<Multiple<Response.Repository>>($"{project}/_apis/git/repositories");
        }

        public static VstsRequest<Multiple<Push>> Pushes(string project, string repositoryId)
        {
            return new VstsRequest<Multiple<Push>>($"/{project}/_apis/git/repositories/{repositoryId}/pushes");
        }
    }
}