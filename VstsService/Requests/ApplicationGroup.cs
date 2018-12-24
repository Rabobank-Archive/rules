using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ApplicationGroup
    {
        public static IVstsRestRequest<Response.ApplicationGroups> ApplicationGroups(string project)
        {
            return new VstsRestRequest<Response.ApplicationGroups>($"{project}/_api/_identity/ReadScopedApplicationGroupsJson?__v=5");
        }
        
        public static IVstsRestRequest<Response.ApplicationGroups> GroupMembers(string project, string groupId)
        {
            return new VstsRestRequest<Response.ApplicationGroups>($"/{project}/_api/_identity/ReadGroupMembers?__v=5&scope={groupId}&readMembers=true");
        }
    }
}
