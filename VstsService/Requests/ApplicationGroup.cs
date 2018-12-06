using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ApplicationGroup
    {
        public static IVstsRestRequest<Response.Multiple<Response.ApplicationGroup>> ApplicationGroups(string project)
        {
            return new VstsRestRequest<Response.Multiple<Response.ApplicationGroup>>($"{project}/_api/_identity/ReadScopedApplicationGroupsJson?__v=5", Method.GET);
        }
    }
}
