using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ApplicationGroup
    {
        public static IVstsRequest<Response.ApplicationGroups> ApplicationGroups(string project)
        {
            return new VstsRequest<Response.ApplicationGroups>($"{project}/_api/_identity/ReadScopedApplicationGroupsJson?__v=5");
        }
        
        public static IVstsRequest<Response.ApplicationGroups> GroupMembers(string project, string groupId)
        {
            return new VstsRequest<Response.ApplicationGroups>($"/{project}/_api/_identity/ReadGroupMembers?__v=5&scope={groupId}&readMembers=true");
        }
        
        public static IVstsRequest<Response.ApplicationGroups> ExplicitIdentitiesRepos(string projectId, string namespaceId)
        {
            return new VstsRequest<Response.ApplicationGroups>(
                $"/{projectId}/_api/_security/ReadExplicitIdentitiesJson?__v=5&permissionSetId={namespaceId}&permissionSetToken=repoV2%2F{projectId}%2F");
        }

        public static IVstsRequest<Response.ApplicationGroups> ExplicitIdentitiesPipelines(string projectId, string namespaceId, string pipelineId)
        {
            return new VstsRequest<Response.ApplicationGroups>(
                $"/{projectId}/_api/_security/ReadExplicitIdentitiesJson?__v=5&permissionSetId={namespaceId}&permissionSetToken={projectId}%2F{pipelineId}");
        }
    }
}
