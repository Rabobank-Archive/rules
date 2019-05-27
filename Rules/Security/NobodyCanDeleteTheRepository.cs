using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheRepository : RuleBase, IRule, IReconcile
    {
        private const string NamespaceId = "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87"; // Git Repositories
        private readonly IVstsRestClient _client;
        protected override string PermissionsDisplayName => "Delete repository";
        protected override IEnumerable<int> AllowedPermissions => new[] { PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited };
        protected override IEnumerable<string> IgnoredIdentitiesDisplayNames => new[] { "Project Collection Administrators", "Project Collection Service Accounts" };

        string IRule.Description => "Nobody can delete the repository";
        string IRule.Why => "To enforce auditability, no data should be deleted. Therefore, nobody should be able to delete the repository.";
        string[] IReconcile.Impact => new[]
        {
            "For all application groups the 'Delete Repository' permission is set to Deny",
            "For all single users the 'Delete Repository' permission is set to Deny"
        };

        public NobodyCanDeleteTheRepository(IVstsRestClient client)
        {
            _client = client;
        }

        protected override IEnumerable<ApplicationGroup> LoadGroups(string projectId, string id) =>
            _client.Get(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesRepos(projectId, NamespaceId)).Identities;

        protected override PermissionsSetId LoadPermissionsSetForGroup(string projectId, string id, ApplicationGroup group) =>
            _client.Get(Permissions.PermissionsGroupRepository(projectId, NamespaceId, group.TeamFoundationId, id));

        protected override void UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission) =>
            _client.Post(Permissions.ManagePermissions(projectId), new Permissions.ManagePermissionsData(@group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission).Wrap());
    }
}