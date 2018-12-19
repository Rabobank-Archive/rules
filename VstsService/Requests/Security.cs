namespace SecurePipelineScan.VstsService.Requests
{
    public static class Security
    {
        public static IVstsRestRequest<Response.Security.IdentityGroup> GroupMembers(string projectId, string groupId)
        {
            return new VstsRestRequest<Response.Security.IdentityGroup>($"/{projectId}/_api/_identity/ReadGroupMembers?__v=5&scope={groupId}&readMembers=true");
        }

        public static IVstsRestRequest<Response.Security.IdentityGroup> Groups(string projectId)
        {
            return new VstsRestRequest<Response.Security.IdentityGroup>($"/{projectId}/_api/_identity/ReadScopedApplicationGroupsJson?__v=5");
        }
    }
}