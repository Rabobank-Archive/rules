using System.Collections.Generic;
using SecurePipelineScan.VstsService;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanManageApprovalsAndCreateReleases : PipelineRuleBase, IRule, IReconcile
    {
        public NobodyCanManageApprovalsAndCreateReleases(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        protected override string NamespaceId => "c788c23e-1b46-4162-8f5e-d7585343b5de"; //release management
        protected override IEnumerable<int> PermissionBits => new[]
        {
            8,      //Manage release approvers
            64      //Create releases
        };
        protected override IEnumerable<int> AllowedPermissions => new[]
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };
        protected override IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators"
        };

        string IRule.Description => "Nobody can both manage approvals and create releases";
        string IRule.Why => "To ensure the four eyes principle, users should not be able to " +
            "remove approvals and thereafter start an unapproved release.";

        string[] IReconcile.Impact => new[]
        {
            "If the Production Environment Owner group does not exist, this group will be created with " +
            "the 'Create Releases' permission set to Deny and the 'Manage Release Approvers' permission set to Allow",
            "Please note that user(s) should be added manually to the Production Environment Owner group",
            "For all other security groups where the 'Create Releases' permission is set to Allow, " +
            "the 'Manage Release Approvers' permission is set to Deny",
        };

        public override async Task<bool> Evaluate(string projectId, string id)
        {
            var groups = (await LoadGroups(projectId, id))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroup(projectId, id, group);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit));

                if (!permissions.Any(p => AllowedPermissions.Contains(p.PermissionId)))
                    return false;
            }
            return true;
        }

        public override async Task Reconcile(string projectId, string id)
        {
            if (!(await LoadGroups(projectId)).Any(g => g.FriendlyDisplayName == "Production Environment Owners"))
            {
                var group = await CreateProductionEnvironmentOwnersGroup(projectId);
                var permissionSetId = await LoadPermissionsSetForGroup(projectId, group);

                var createReleasesPermission = permissionSetId.Permissions.SingleOrDefault(p => p.PermissionBit == 64);
                createReleasesPermission.PermissionId = PermissionId.Deny;
                await UpdatePermission(projectId, group, permissionSetId, createReleasesPermission);

                var manageApprovalsPermission = permissionSetId.Permissions.SingleOrDefault(p => p.PermissionBit == 8);
                manageApprovalsPermission.PermissionId = PermissionId.Allow;
                await UpdatePermission(projectId, group, permissionSetId, manageApprovalsPermission);
            }

            var groups = (await LoadGroups(projectId, id))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroup(projectId, id, group);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit));

                if (!permissions.Any(p => AllowedPermissions.Contains(p.PermissionId)))
                {
                    var permissionToUpdate = new Permission();
                    if (group.FriendlyDisplayName == "Production Environment Owners")
                        permissionToUpdate = permissions.SingleOrDefault(p => p.PermissionBit == 64);//Create Releases
                    else
                        permissionToUpdate = permissions.SingleOrDefault(p => p.PermissionBit == 8);//Manage Approvers
                    permissionToUpdate.PermissionId = PermissionId.Deny;
                    await UpdatePermission(projectId, group, permissionSetId, permissionToUpdate);
                }
            }
        }
    }
}