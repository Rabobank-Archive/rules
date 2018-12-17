using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ApplicationGroup
    {
        public static IVstsRestRequest<Response.ApplicationGroups> ApplicationGroups(string project)
        {
            return new VstsRestRequest<Response.ApplicationGroups>($"{project}/_api/_identity/ReadScopedApplicationGroupsJson?__v=5");
        }
    }
}
