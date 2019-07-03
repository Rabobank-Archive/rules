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

        const int ManageApprovalsPermissionBit = 8;
        const int CreateReleasesPermissionBit = 64;

        protected override string NamespaceId => "c788c23e-1b46-4162-8f5e-d7585343b5de"; //release management
        protected override IEnumerable<int> PermissionBits => new[]
        {
            ManageApprovalsPermissionBit,      
            CreateReleasesPermissionBit 
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

        public override async Task<bool> Evaluate(string projectId, string releasePipelineId)
        {
            var groups = (await LoadGroups(projectId, releasePipelineId))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroup(projectId, releasePipelineId, group);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit));

                if (!permissions.Any(p => AllowedPermissions.Contains(p.PermissionId)))
                    return false;
            }
            return true;
        }

        public override async Task Reconcile(string projectId, string releasePipelineId)
        {
            if ((await LoadGroups(projectId)).All(g => g.FriendlyDisplayName != "Production Environment Owners"))
            {
                var group = await CreateProductionEnvironmentOwnersGroup(projectId);
                var permissionSetId = await LoadPermissionsSetForGroup(projectId, group);

                var createReleasesPermission = permissionSetId.Permissions.Single(p => p.PermissionBit == CreateReleasesPermissionBit);
                createReleasesPermission.PermissionId = PermissionId.Deny;
                await UpdatePermission(projectId, group, permissionSetId, createReleasesPermission);

                var manageApprovalsPermission = permissionSetId.Permissions.Single(p => p.PermissionBit == ManageApprovalsPermissionBit);
                manageApprovalsPermission.PermissionId = PermissionId.Allow;
                await UpdatePermission(projectId, group, permissionSetId, manageApprovalsPermission);
            }

            var groups = (await LoadGroups(projectId, releasePipelineId))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroup(projectId, releasePipelineId, group);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit))
                    .ToList();

                if (permissions.Any(p => AllowedPermissions.Contains(p.PermissionId)))
                    continue;

                Permission permissionToUpdate;
                if (group.FriendlyDisplayName == "Production Environment Owners")
                    permissionToUpdate = permissions.Single(p => p.PermissionBit == CreateReleasesPermissionBit);
                else
                    permissionToUpdate = permissions.Single(p => p.PermissionBit == ManageApprovalsPermissionBit);
                permissionToUpdate.PermissionId = PermissionId.Deny;
                await UpdatePermission(projectId, group, permissionSetId, permissionToUpdate);
            }
        }
    }
}