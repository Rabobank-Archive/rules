using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using PermissionsSetId = SecurePipelineScan.VstsService.Response.PermissionsSetId;
using System;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheRepository : RuleBase, IRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        const int PermissionBitDeletRepository = 512;
        const int PermissionBitManagePermissions = 8192;

        protected override string NamespaceId => "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87"; //Git Repositories
        protected override IEnumerable<int> PermissionBits => new[]
{
            PermissionBitDeletRepository,
            PermissionBitManagePermissions
        };
        protected override IEnumerable<int> AllowedPermissions => new[]
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };
        protected override IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators",
            "Project Collection Service Accounts"
        };

        string IRule.Description => "Nobody can delete the repository";
        string IRule.Why => "To enforce auditability, no data should be deleted. " +
            "Therefore, nobody should be able to delete the repository.";
        bool IRule.IsSox => true;
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Repository' permission is set to Deny",
            "For all security groups the 'Manage Permissions' permission is set to Deny"
        };

        public NobodyCanDeleteTheRepository(IVstsRestClient client)
        {
            _client = client;
        }

        protected override async Task<IEnumerable<ApplicationGroup>> LoadGroupsAsync(string projectId, string repositoryId) =>
            (await _client.GetAsync(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesRepos(projectId, NamespaceId, repositoryId))
                .ConfigureAwait(false))
                .Identities;

        protected override Task<PermissionsSetId> LoadPermissionsSetForGroupAsync(string projectId, string repositoryId,
            ApplicationGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return LoadPermissionsSetForGroupInternalAsync(projectId, repositoryId, group);
        }

        private async Task<PermissionsSetId> LoadPermissionsSetForGroupInternalAsync(string projectId, string repositoryId, ApplicationGroup group)
        {
            return (await _client.GetAsync(Permissions.PermissionsGroupRepository(projectId, NamespaceId, group.TeamFoundationId, repositoryId))
                        .ConfigureAwait(false));
        }

        protected override Task UpdatePermissionAsync(string projectId, ApplicationGroup group,
            PermissionsSetId permissionSetId, Response.Permission permission)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (permissionSetId == null)
                throw new ArgumentNullException(nameof(permissionSetId));

            return UpdatePermissionInternalAsync(projectId, group, permissionSetId, permission);
        }

        private async Task UpdatePermissionInternalAsync(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Response.Permission permission)
        {
            await _client.PostAsync(Permissions.ManagePermissions(projectId),
                new Permissions.ManagePermissionsData(group.TeamFoundationId, permissionSetId.DescriptorIdentifier,
                    permissionSetId.DescriptorIdentityType, permission).Wrap())
                .ConfigureAwait(false);
        }
    }
}