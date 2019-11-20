using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteReleases : IReleasePipelineRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteReleases(IVstsRestClient client)
        {
            _client = client;
        }

        private const int PermissionBitDeleteReleasePipelines = 4;
        private const int PermissionBitAdministerReleasePermissions = 512;
        private const int PermissionBitDeleteReleases = 1024;

        private static string NamespaceId => "c788c23e-1b46-4162-8f5e-d7585343b5de"; //release management

        private static IEnumerable<int> PermissionBits => new[]
        {
            PermissionBitDeleteReleasePipelines,
            PermissionBitAdministerReleasePermissions,
            PermissionBitDeleteReleases
        };

        private static IEnumerable<int> AllowedPermissions => new[]
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };

        private static IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators"
        };

        string IRule.Description => "Nobody can delete releases (SOx)";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/9I8AD";
        bool IRule.IsSox => true;
        bool IReconcile.RequiresStageId => false;
        
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Releases' permission is set to Deny",
            "For all security groups the 'Delete Release Pipeline' permission is set to Deny",
            "For all security groups the 'Administer Release Permissions' permission is set to Deny"
        };

        public async Task<bool?> EvaluateAsync(string projectId, string stageId, Response.ReleaseDefinition releasePipeline)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            var groups = (await _client.GetAsync(ApplicationGroup.ExplicitIdentitiesPipelines(projectId, NamespaceId, releasePipeline.Id)).ConfigureAwait(false))
                .Identities.Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            var permissionsSetIds = await Task.WhenAll(
                groups.Select(g => _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(projectId, NamespaceId, g.TeamFoundationId, releasePipeline.Id)))).ConfigureAwait(false);
            
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