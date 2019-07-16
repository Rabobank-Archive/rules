using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using PermissionsSetId = SecurePipelineScan.VstsService.Response.PermissionsSetId;

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
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Repository' permission is set to Deny",
            "For all security groups the 'Manage Permissions' permission is set to Deny"
        };

        public NobodyCanDeleteTheRepository(IVstsRestClient client)
        {
            _client = client;
        }

        protected override async Task<IEnumerable<ApplicationGroup>> LoadGroupsAsync(string projectId, string id) =>
            (await _client.GetAsync(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesRepos(projectId, NamespaceId))
                .ConfigureAwait(false))
                .Identities;

        protected override async Task<PermissionsSetId> LoadPermissionsSetForGroupAsync(string projectId, string id,
            ApplicationGroup group) =>
            (await _client.GetAsync(Permissions.PermissionsGroupRepository(projectId, NamespaceId, group?.TeamFoundationId, id))
            .ConfigureAwait(false));

        protected override async Task UpdatePermissionAsync(string projectId, ApplicationGroup group, 
            PermissionsSetId permissionSetId, Response.Permission permission) =>
            await _client.PostAsync(Permissions.ManagePermissions(projectId), 
                new Permissions.ManagePermissionsData(group?.TeamFoundationId, permissionSetId?.DescriptorIdentifier, 
                    permissionSetId?.DescriptorIdentityType, permission).Wrap())
            .ConfigureAwait(false);
    }
}