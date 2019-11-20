using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteBuilds : IBuildPipelineRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteBuilds(IVstsRestClient client)
        {
            _client = client;
        }

        private const int PermissionBitDeleteBuilds = 8;
        private const int PermissionBitDestroyBuilds = 32;
        private const int PermissionBitDeleteBuildDefinition = 4096;
        private const int PermissionBitAdministerBuildPermissions = 16384;

        private static string NamespaceId => "33344d9c-fc72-4d6f-aba5-fa317101a7e9"; //build

        private static IEnumerable<int> PermissionBits => new[]
        {
            PermissionBitDeleteBuilds,
            PermissionBitDestroyBuilds,
            PermissionBitDeleteBuildDefinition,
            PermissionBitAdministerBuildPermissions
        };

        private static IEnumerable<int> AllowedPermissions => new[]
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };

        private static IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators",
            "Project Collection Build Administrators"
        };

        string IRule.Description => "Nobody can delete builds (SOx)";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/V48AD";
        bool IRule.IsSox => true;
        bool IReconcile.RequiresStageId => false;
        
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Builds' permission is set to Deny",
            "For all security groups the 'Destroy Builds' permission is set to Deny",
            "For all security groups the 'Delete Build Definitions' permission is set to Deny",
            "For all security groups the 'Administer Build Permissions' permission is set to Deny"
        };

        public async Task<bool?> EvaluateAsync(Response.Project project, Response.BuildDefinition buildPipeline)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            var groups = (await _client.GetAsync(ApplicationGroup.ExplicitIdentitiesPipelines(project.Id, NamespaceId, buildPipeline.Id)).ConfigureAwait(false))
                .Identities.Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            var permissionsSetIds = await Task.WhenAll(
                groups.Select(g => _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(project.Id, NamespaceId, g.TeamFoundationId, buildPipeline.Id)))).ConfigureAwait(false);

            return permissionsSetIds
                .SelectMany(p => p.Permissions)
                .All(p => !PermissionBits.Contains(p.PermissionBit) || AllowedPermissions.Contains(p.PermissionId));
        }

        public async Task ReconcileAsync(string projectId, string itemId, string stageId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            var groups = (await _client.GetAsync(ApplicationGroup.ExplicitIdentitiesPipelines(projectId, NamespaceId, itemId)).ConfigureAwait(false))
                .Identities.Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(projectId, NamespaceId, group.TeamFoundationId, itemId)).ConfigureAwait(false);
                var permissions = permissionSetId.Permissions.Where(p => PermissionBits.Contains(p.PermissionBit)
                                                                         && !AllowedPermissions.Contains(p.PermissionId));

                foreach (var permission in permissions)
                {
                    permission.PermissionId = PermissionId.Deny;
                    await _client.PostAsync(
                            Permissions.ManagePermissions(projectId),
                            new Permissions.ManagePermissionsData(group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission).Wrap())
                        .ConfigureAwait(false);
                }
            }
        }
    }
}