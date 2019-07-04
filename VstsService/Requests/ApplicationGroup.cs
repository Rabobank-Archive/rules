
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ApplicationGroup
    {
        public static IVstsRequest<Response.ApplicationGroups> ApplicationGroups(string project)
        {
            return new VstsRequest<Response.ApplicationGroups>(
                $"{project}/_api/_identity/ReadScopedApplicationGroupsJson", new Dictionary<string, object>
                {
                    {"__v", "5"}
                });
        }
        
        public static IVstsRequest<Response.ApplicationGroups> GroupMembers(string project, string groupId)
        {
            return new VstsRequest<Response.ApplicationGroups>($"/{project}/_api/_identity/ReadGroupMembers", new Dictionary<string, object>
            {
                {"__v", "5"},
                {"scope", groupId},
                {"readMembers", "true"}
            });
        }
        
        public static IVstsRequest<Response.ApplicationGroups> ExplicitIdentitiesRepos(string projectId, string namespaceId)
        {
            return new VstsRequest<Response.ApplicationGroups>(
                $"/{projectId}/_api/_security/ReadExplicitIdentitiesJson", new Dictionary<string, object>
                {
                    {"__v", "5"},
                    {"permissionSetId", namespaceId},
                    {"permissionSetToken", $"repoV2/{projectId}/"}
                });
        }

        public static IVstsRequest<Response.ApplicationGroups> ExplicitIdentitiesPipelines(string projectId, string namespaceId, string pipelineId)
        {
            return new VstsRequest<Response.ApplicationGroups>(
                $"/{projectId}/_api/_security/ReadExplicitIdentitiesJson", new Dictionary<string, object>
                {
                    {"__v", "5"},
                    {"permissionSetId", namespaceId},
                    {"permissionSetToken", $"{projectId}/{pipelineId}"}
                });
        }

        public static IVstsRequest<Response.ApplicationGroups> ExplicitIdentitiesPipelines(string projectId, string namespaceId)
        {
            return new VstsRequest<Response.ApplicationGroups>(
                $"/{projectId}/_api/_security/ReadExplicitIdentitiesJson", new Dictionary<string, object>
                {
                    {"__v", "5"},
                    {"permissionSetId", namespaceId},
                    {"permissionSetToken", $"{projectId}"}
                });
        }
    }
}
