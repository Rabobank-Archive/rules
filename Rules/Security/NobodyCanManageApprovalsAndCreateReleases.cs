using System.Collections.Generic;
using SecurePipelineScan.VstsService;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using SecurePipelineScan.VstsService.Response;
using Request = SecurePipelineScan.VstsService.Requests;
using System;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanManageApprovalsAndCreateReleases : ItemHasPermissionRuleBase, IReleasePipelineRule, IReconcile
    {
        readonly IVstsRestClient _client;

        public NobodyCanManageApprovalsAndCreateReleases(IVstsRestClient client) : base(client)
        {
            _client = client;
        }

        private const int ManageApprovalsPermissionBit = 8;
        private const int CreateReleasesPermissionBit = 64;

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

        string IRule.Description => "Nobody can both manage approvals and create releases (SOx)";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/1o8AD";
        bool IRule.IsSox => true;
        bool IReconcile.RequiresStageId => false;
        string[] IReconcile.Impact => new[]
        {
            "If the Production Environment Owner group does not exist, this group will be created with " +
            "the 'Create Releases' permission set to Deny and the 'Manage Release Approvers' permission set to Allow",
            "Please note that user(s) should be added manually to the Production Environment Owner group",
            "For all other security groups where the 'Create Releases' permission is set to Allow, " +
            "the 'Manage Release Approvers' permission is set to Deny",
        };
        public async Task<bool?> EvaluateAsync(string projectId, string stageId, ReleaseDefinition releasePipeline)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            return await EvaluateAsync(projectId, releasePipeline.Id, RuleScopes.ReleasePipelines)
                .ConfigureAwait(false);
        }

        public override async Task<bool> EvaluateAsync(string projectId, string itemId, string scope)
        {
            var groups = (await LoadGroupsAsync(projectId, itemId, scope).ConfigureAwait(false))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroupAsync(projectId, itemId, group, scope)
                    .ConfigureAwait(false);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit));

                if (!permissions.Any(p => AllowedPermissions.Contains(p.PermissionId)))
                    return false;
            }
            return true;
        }

        public async Task ReconcileAsync(string projectId, string itemId, string scope, string stageId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            await ReconcileAsync(projectId, itemId, scope)
                .ConfigureAwait(false);
        }

        public override async Task ReconcileAsync(string projectId, string itemId, string scope)
        {
            var projectGroups = await LoadGroupsAsync(projectId)
                .ConfigureAwait(false);

            if (projectGroups.All(g => g.FriendlyDisplayName != "Production Environment Owners"))
            {
                await CreateProductionEnvironmentOwnerGroupAsync(projectId, scope)
                    .ConfigureAwait(false);
            }

            var groups = (await LoadGroupsAsync(projectId, itemId, scope)
                .ConfigureAwait(false))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroupAsync(projectId, itemId, group, scope)
                    .ConfigureAwait(false);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit))
                    .ToList();

                if (permissions.Any(p => AllowedPermissions.Contains(p.PermissionId)))
                    continue;

                var permissionToUpdate = group.FriendlyDisplayName == "Production Environment Owners"
                    ? permissions.Single(p => p.PermissionBit == CreateReleasesPermissionBit)
                    : permissions.Single(p => p.PermissionBit == ManageApprovalsPermissionBit);
                permissionToUpdate.PermissionId = PermissionId.Deny;
                await UpdatePermissionAsync(projectId, group, permissionSetId, permissionToUpdate)
                    .ConfigureAwait(false);
            }
        }

        private async Task CreateProductionEnvironmentOwnerGroupAsync(string projectId, string scope)
        {
            var peoGroup = await _client.PostAsync(Request.Security.ManageGroup(projectId),
                    new Request.Security.ManageGroupData
                    {
                        Name = "Production Environment Owners"
                    })
                .ConfigureAwait(false);

            var permissionSetId = await LoadPermissionsSetForGroupAsync(projectId, peoGroup, scope)
                .ConfigureAwait(false);

            var createReleasesPermission = permissionSetId.Permissions.Single(p => p.PermissionBit == CreateReleasesPermissionBit);
            createReleasesPermission.PermissionId = PermissionId.Deny;
            await UpdatePermissionAsync(projectId, peoGroup, permissionSetId, createReleasesPermission)
                .ConfigureAwait(false);

            var manageApprovalsPermission = permissionSetId.Permissions.Single(p => p.PermissionBit == ManageApprovalsPermissionBit);
            manageApprovalsPermission.PermissionId = PermissionId.Allow;
            await UpdatePermissionAsync(projectId, peoGroup, permissionSetId, manageApprovalsPermission)
                .ConfigureAwait(false);
        }
    }
}