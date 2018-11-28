using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Repository
    {
        public static IVstsRestRequest<Response.Multiple<Response.Repository>> Repositories(string project)
        {
            return new VstsRestRequest<Response.Multiple<Response.Repository>>($"{project}/_apis/git/repositories", Method.GET);
        }
    }
}