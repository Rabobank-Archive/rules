using System.Collections.Generic;
using SecurePipelineScan.VstsService;
using System.Linq;
using System.Threading.Tasks;
using Response = SecurePipelineScan.VstsService.Response;
using Request = SecurePipelineScan.VstsService.Requests;
using System;
using System.Diagnostics.CodeAnalysis;
using SecurePipelineScan.Rules.PermissionBits;
using SecurePipelineScan.VstsService.Permissions;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanManageApprovalsAndCreateReleases : IReleasePipelineRule, IReconcile
    {
        readonly IVstsRestClient _client;

        public NobodyCanManageApprovalsAndCreateReleases(IVstsRestClient client)
        {
            _client = client;
        }

        private const string PeoGroupName = "Production Environment Owners";

        private static IEnumerable<int> PermissionBits => new[]
        {
            Release.ManageApprovalsPermissionBit,
            Release.CreateReleasesPermissionBit
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

        [ExcludeFromCodeCoverage] string IRule.Description => "Nobody can both manage approvals and create releases (SOx)";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/1o8AD";

        [ExcludeFromCodeCoverage]
        string[] IReconcile.Impact => new[]
        {
            "If the Production Environment Owner group does not exist, this group will be created with " +
            "the 'Create Releases' permission set to Deny and the 'Manage Release Approvers' permission set to Allow",
            "Please note that user(s) should be added manually to the Production Environment Owner group",
            "For all other security groups where the 'Create Releases' permission is set to Allow, " +
            "the 'Manage Release Approvers' permission is set to NotSet",
        };
        public async Task<bool?> EvaluateAsync(string projectId, Response.ReleaseDefinition releasePipeline)
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

        public async Task ReconcileAsync(string projectId, string itemId)
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
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName))
                .ToList();

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

                if (IsPeoGroup(group))
                    await UpdatePermissionAsync(projectId, group, permissionSetId,
                            Release.CreateReleasesPermissionBit, PermissionId.Deny)
                        .ConfigureAwait(false);
                else
                    await UpdatePermissionAsync(projectId, group, permissionSetId,
                            Release.ManageApprovalsPermissionBit, PermissionId.NotSet)
                        .ConfigureAwait(false);
            }
        }

        private async Task CreateProductionEnvironmentOwnerGroupIfNotExistsAsync(string projectId,
            IEnumerable<Response.ApplicationGroup> projectGroups, IEnumerable<Response.ApplicationGroup> groups)
        {
            var peoGroup = projectGroups.FirstOrDefault(g => IsPeoGroup(g)) ??
                await CreateProductionEnvironmentOwnerGroupAsync(projectId)
                    .ConfigureAwait(false);
            if (!groups.Any(g => IsPeoGroup(g)))
                await UpdatePermissionsProductionEnvironmentOwnerGroupAsync(projectId, peoGroup)
                    .ConfigureAwait(false);
        }

        private static bool IsPeoGroup(Response.ApplicationGroup group)
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

            await UpdatePermissionAsync(projectId, peoGroup, permissions, Release.CreateReleasesPermissionBit, PermissionId.Deny)
                .ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissions, Release.ManageApprovalsPermissionBit, PermissionId.Allow)
                .ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissions, Release.ViewReleasePipeline, PermissionId.Allow)
                .ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissions, Release.EditReleasePipeline, PermissionId.Allow)
                .ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissions, Release.EditReleaseStage, PermissionId.Allow)
                .ConfigureAwait(false);
        }

        private async Task UpdatePermissionAsync(string projectId, Response.ApplicationGroup @group,
            Response.PermissionsSetId permissions, int bit, int to)
        {
            var permission = permissions.Permissions.Single(p => p.PermissionBit == bit);
            permission.PermissionId = to;

            await _client.PostAsync(Request.Permissions.ManagePermissions(projectId),
                    new ManagePermissionsData(@group.TeamFoundationId, permissions.DescriptorIdentifier,
                    permissions.DescriptorIdentityType, permission.PermissionToken, permission).Wrap())
                .ConfigureAwait(false);
        }

        private static string ExtractToken(string projectId, string releasePipelineId, string releasePipelinePath) =>
            releasePipelinePath == "\\"
                ? $"{projectId}/{releasePipelineId}"
                : $"{projectId}{releasePipelinePath.Replace("\\", "/")}/{releasePipelineId}";
    }
}