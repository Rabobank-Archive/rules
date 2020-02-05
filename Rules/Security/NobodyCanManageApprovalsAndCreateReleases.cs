using System.Collections.Generic;
using SecurePipelineScan.VstsService;
using System.Linq;
using System.Threading.Tasks;
using Response = SecurePipelineScan.VstsService.Response;
using Request = SecurePipelineScan.VstsService.Requests;
using System;
using SecurePipelineScan.Rules.Permissions;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanManageApprovalsAndCreateReleases : IReleasePipelineRule, IReconcile
    {
        readonly IVstsRestClient _client;

        public NobodyCanManageApprovalsAndCreateReleases(IVstsRestClient client)
        {
            _client = client;
        }

        private const int ManageApprovalsPermissionBit = 8;
        private const int CreateReleasesPermissionBit = 64;
        private const string PeoGroupName = "Production Environment Owners";

        private static IEnumerable<int> PermissionBits => new[]
        {
            ManageApprovalsPermissionBit,
            CreateReleasesPermissionBit
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
        public async Task<bool?> EvaluateAsync(string projectId, string stageId, Response.ReleaseDefinition releasePipeline)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            var groups = (await _client.GetAsync(
                    Request.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, SecurityNamespaceIds.Release, releasePipeline.Id)).ConfigureAwait(false))
                .Identities
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetToken = ExtractToken(projectId, releasePipeline.Id, releasePipeline.Path);
                var permissionSetId = await _client.GetAsync(Request.Permissions.PermissionsGroupSetIdDefinition(
                        projectId, SecurityNamespaceIds.Release, group.TeamFoundationId, permissionSetToken))
                    .ConfigureAwait(false);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit));

                if (!permissions.Any(p => AllowedPermissions.Contains(p.PermissionId)))
                    return false;
            }

            return true;
        }

        public async Task ReconcileAsync(string projectId, string itemId, string stageId, string userId, object data = null)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            var releasePipeline = await _client.GetAsync(Request.ReleaseManagement.Definition(projectId, itemId))
                .ConfigureAwait(false);

            var projectGroups = (await _client.GetAsync(Request.ApplicationGroup.ApplicationGroups(projectId))
                .ConfigureAwait(false))
                .Identities;

            var groups = (await _client.GetAsync(Request.ApplicationGroup.ExplicitIdentitiesPipelines(
                    projectId, SecurityNamespaceIds.Release, itemId)).ConfigureAwait(false))
                .Identities
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            await CreateProductionEnvironmentOwnerGroupIfNotExistsAsync(projectId, projectGroups, groups)
                .ConfigureAwait(false);

            foreach (var group in groups)
            {
                var permissionSetToken = ExtractToken(projectId, releasePipeline.Id, releasePipeline.Path);
                var permissionSetId = await _client.GetAsync(Request.Permissions.PermissionsGroupSetIdDefinition(
                        projectId, SecurityNamespaceIds.Release, group.TeamFoundationId, permissionSetToken))
                    .ConfigureAwait(false);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit))
                    .ToList();

                if (permissions.Any(p => AllowedPermissions.Contains(p.PermissionId)))
                    continue;

                var permissionToUpdate = PeoGroupExists(group)
                    ? CreateReleasesPermissionBit
                    : ManageApprovalsPermissionBit;

                await UpdatePermissionAsync(projectId, group, permissionSetId, permissionToUpdate, PermissionId.Deny)
                    .ConfigureAwait(false);
            }
        }

        private async Task CreateProductionEnvironmentOwnerGroupIfNotExistsAsync(string projectId,
            IEnumerable<Response.ApplicationGroup> projectGroups, IEnumerable<Response.ApplicationGroup> groups)
        {
            var peoGroup = projectGroups.FirstOrDefault(g => PeoGroupExists(g)) ??
                await CreateProductionEnvironmentOwnerGroupAsync(projectId)
                    .ConfigureAwait(false);
            if (!groups.Any(g => PeoGroupExists(g)))
                await UpdatePermissionsProductionEnvironmentOwnerGroupAsync(projectId, peoGroup)
                    .ConfigureAwait(false);
        }

        private static bool PeoGroupExists(Response.ApplicationGroup group)
            => group.FriendlyDisplayName == PeoGroupName;

        private async Task<Response.ApplicationGroup> CreateProductionEnvironmentOwnerGroupAsync(string projectId) =>
            await _client.PostAsync(Request.Security.ManageGroup(projectId),
                    new Request.Security.ManageGroupData { Name = PeoGroupName })
                .ConfigureAwait(false);

        private async Task UpdatePermissionsProductionEnvironmentOwnerGroupAsync(string projectId,
            Response.ApplicationGroup peoGroup)
        {
            var permissions = await _client.GetAsync(Request.Permissions.PermissionsGroupSetId(
                    projectId, SecurityNamespaceIds.Release, peoGroup.TeamFoundationId))
                .ConfigureAwait(false);

            await UpdatePermissionAsync(projectId, peoGroup, permissions, CreateReleasesPermissionBit, PermissionId.Deny)
                .ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissions, ManageApprovalsPermissionBit, PermissionId.Allow)
                .ConfigureAwait(false);
        }

        private async Task UpdatePermissionAsync(string projectId, Response.ApplicationGroup @group,
            Response.PermissionsSetId permissions, int bit, int to)
        {
            var permission = permissions.Permissions.Single(p => p.PermissionBit == bit);
            permission.PermissionId = to;

            await _client.PostAsync(Request.Permissions.ManagePermissions(projectId),
                    new Request.ManagePermissionsData(@group.TeamFoundationId, permissions.DescriptorIdentifier,
                    permissions.DescriptorIdentityType, permission.PermissionToken, permission).Wrap())
                .ConfigureAwait(false);
        }

        private static string ExtractToken(string projectId, string releasePipelineId, string releasePipelinePath) =>
            releasePipelinePath == "\\"
                ? $"{projectId}/{releasePipelineId}"
                : $"{projectId}{releasePipelinePath.Replace("\\", "/")}/{releasePipelineId}";
    }
}