using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheBuildPipeline
    {
        private const string DeleteBuildPipeline = "Delete build definition";
        private static readonly int[] AllowedPermissions = { PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited };

        private readonly IVstsRestClient _client;
        private readonly string _namespaceBuild;

        public NobodyCanDeleteTheBuildPipeline(IVstsRestClient client)
        {
            _client = client;
            _namespaceBuild = _client
                .Get(VstsService.Requests.SecurityNamespace.SecurityNamespaces())
                .First(s => s.Name == "Build").NamespaceId;
        }

        public bool Evaluate(string project, string buildPipelineId)
        {
            var projectId =
                _client.Get(VstsService.Requests.Project.Properties(project)).Id;

            var namespaceBuild =
                _client.Get(VstsService.Requests.SecurityNamespace.SecurityNamespaces())
                    .First(s => s.Name == "Build").NamespaceId;

            var groups =
                _client.Get(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, namespaceBuild, buildPipelineId))
                    .Identities
                    .Where(g => g.FriendlyDisplayName != "Project Collection Administrators" &&
                                g.FriendlyDisplayName != "Project Collection Build Administrators" &&
                                g.FriendlyDisplayName != "Project Collection Service Accounts");

            return CheckNoBodyCanDeleteBuildPipeline(projectId, namespaceBuild, buildPipelineId, groups);
        }

        private bool CheckNoBodyCanDeleteBuildPipeline(string projectId, string namespaceBuild, string buildPipelineId,
            IEnumerable<ApplicationGroup> groups)
        {
            var permissions =
                groups.SelectMany(g => _client.Get(Permissions.PermissionsGroupSetIdDefinition(
                    projectId, namespaceBuild, g.TeamFoundationId, buildPipelineId)).Permissions);

            return permissions.All(p => p.DisplayName != DeleteBuildPipeline || AllowedPermissions.Contains(p.PermissionId));
        }
    }
}