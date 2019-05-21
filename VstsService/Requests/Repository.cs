using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Repository
    {
        public static IVstsRequest<Response.Multiple<Response.Repository>> Repositories(string project)
        {
            return new VstsRequest<Response.Multiple<Response.Repository>>($"{project}/_apis/git/repositories");
        }
    }
}