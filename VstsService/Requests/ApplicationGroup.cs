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
        
        public static IVstsRestRequest<Response.ApplicationGroups> ExplicitIdentitiesRepos(string projectId, string namespaceId)
        {
            return new VstsRestRequest<Response.ApplicationGroups>(
                $"/{projectId}/_api/_security/ReadExplicitIdentitiesJson?__v=5&permissionSetId={namespaceId}&permissionSetToken=repoV2%2F{projectId}%2F");
        }

        public static IVstsRestRequest<Response.ApplicationGroups> ExplicitIdentitiesPipelines(string projectId, string namespaceId, string pipelineId)
        {
            return new VstsRestRequest<Response.ApplicationGroups>(
                $"/{projectId}/_api/_security/ReadExplicitIdentitiesJson?__v=5&permissionSetId={namespaceId}&permissionSetToken={projectId}%2F{pipelineId}");
        }
    }
}
