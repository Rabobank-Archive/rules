using SecurePipelineScan.Rules.PermissionBits;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
using static SecurePipelineScan.VstsService.Requests.Security;
using Response = SecurePipelineScan.VstsService.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanManageApprovalsAndCreateReleases : IReleasePipelineRule, IReconcile
    {
        private const string PeoGroupName = "Production Environment Owners";
        private readonly IVstsRestClient _client;

        public NobodyCanManageApprovalsAndCreateReleases(IVstsRestClient client) => _client = client;

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

        [ExcludeFromCodeCoverage] string IRule.Description => 
            "Nobody can both manage approvals and create releases (SOx)";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/1o8AD";
        [ExcludeFromCodeCoverage] string[] IReconcile.Impact => new[]
        {
            "If the Production Environment Owner group does not exist, this group will be created with the " +
            "'Create Releases' permission set to Deny and the 'Manage Release Approvers' permission set to Allow",
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

            var groups = await GetReleasePipelineGroupsAsync(projectId, releasePipeline)
                .ConfigureAwait(false);

            foreach (var group in groups)
            {
                var permissions = await GetFilteredPermissionsAsync(projectId, releasePipeline, group)
                    .ConfigureAwait(false);

                if (!HasCorrectPermissions(permissions))
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

            var releasePipeline = await _client.GetAsync(ReleaseManagement.Definition(projectId, itemId))
                .ConfigureAwait(false);
            var projectGroups = await GetProjectGroupsAsync(projectId)
                .ConfigureAwait(false);
            var groups = await GetReleasePipelineGroupsAsync(projectId, releasePipeline)
                .ConfigureAwait(false);

            await CreatePeoGroupIfNotExistsAsync(projectId, projectGroups, groups, releasePipeline)
                .ConfigureAwait(false);

            foreach (var group in groups)
            {
                var permissions = await GetFilteredPermissionsAsync(projectId, releasePipeline, group)
                    .ConfigureAwait(false);
                if (HasCorrectPermissions(permissions))
                    continue;

                var permissionSetId = await GetPermissionSetIdAsync(projectId, releasePipeline, group)
                    .ConfigureAwait(false);
                if (IsPeoGroup(group))
                    await UpdatePermissionAsync(projectId, group, permissionSetId,
                            Release.CreateReleasesPermissionBit, PermissionId.Deny).ConfigureAwait(false);
                else
                    await UpdatePermissionAsync(projectId, group, permissionSetId,
                            Release.ManageApprovalsPermissionBit, PermissionId.NotSet).ConfigureAwait(false);
            }
        }



        private async Task<IEnumerable<Response.ApplicationGroup>> GetProjectGroupsAsync(string projectId) =>
            (await _client.GetAsync(ApplicationGroup.ApplicationGroups(projectId))
                .ConfigureAwait(false))
                .Identities;

        private async Task<IEnumerable<Response.ApplicationGroup>> GetReleasePipelineGroupsAsync(
                string projectId, Response.ReleaseDefinition releasePipeline) =>
            (await _client.GetAsync(ApplicationGroup.ExplicitIdentitiesPipelines(
                    projectId, SecurityNamespaceIds.Release, releasePipeline.Id)).ConfigureAwait(false))
                .Identities
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

        private async Task<IEnumerable<Response.Permission>> GetFilteredPermissionsAsync(
            string projectId, Response.ReleaseDefinition releasePipeline, Response.ApplicationGroup group)
        {
            var permissionSetId = await GetPermissionSetIdAsync(projectId, releasePipeline, group)
                .ConfigureAwait(false);
            return permissionSetId.Permissions
                .Where(p => PermissionBits.Contains(p.PermissionBit));
        }

        private async Task<Response.PermissionsSetId> GetPermissionSetIdAsync(
            string projectId, Response.ReleaseDefinition releasePipeline, Response.ApplicationGroup group)
        {
            var permissionSetToken = ExtractToken(projectId, releasePipeline.Id, releasePipeline.Path);
            return await _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(
                    projectId, SecurityNamespaceIds.Release, group.TeamFoundationId, permissionSetToken))
                .ConfigureAwait(false);
        }

        private async Task CreatePeoGroupIfNotExistsAsync(string projectId, 
            IEnumerable<Response.ApplicationGroup> projectGroups, IEnumerable<Response.ApplicationGroup> groups, 
            Response.ReleaseDefinition releasePipeline)
        {
            var peoGroup = projectGroups.FirstOrDefault(g => IsPeoGroup(g)) ??
                await CreatePeoGroupAsync(projectId).ConfigureAwait(false);
            if (!PeoGroupExists(groups))
                await SetInitialPermissionsPeoGroupAsync(projectId, peoGroup, releasePipeline)
                    .ConfigureAwait(false);
        }

        private static bool PeoGroupExists(IEnumerable<Response.ApplicationGroup> groups) => 
            groups.Any(g => IsPeoGroup(g));

        private static bool IsPeoGroup(Response.ApplicationGroup group) => 
            group.FriendlyDisplayName == PeoGroupName;

        private static bool HasCorrectPermissions(IEnumerable<Response.Permission> permissions) =>
            permissions.Any(p => AllowedPermissions.Contains(p.PermissionId));

        private async Task<Response.ApplicationGroup> CreatePeoGroupAsync(string projectId) =>
            await _client.PostAsync(ManageGroup(projectId), new ManageGroupData { Name = PeoGroupName })
                .ConfigureAwait(false);

        private async Task SetInitialPermissionsPeoGroupAsync(string projectId,
            Response.ApplicationGroup peoGroup, Response.ReleaseDefinition releasePipeline)
        {
            var permissionSetId = await GetPermissionSetIdAsync(projectId, releasePipeline, peoGroup)
                .ConfigureAwait(false);

            await UpdatePermissionAsync(projectId, peoGroup, permissionSetId,
                Release.CreateReleasesPermissionBit, PermissionId.Deny).ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissionSetId,
                Release.ManageApprovalsPermissionBit, PermissionId.Allow).ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissionSetId,
                Release.ViewReleasePipeline, PermissionId.Allow).ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissionSetId,
                Release.EditReleasePipeline, PermissionId.Allow).ConfigureAwait(false);
            await UpdatePermissionAsync(projectId, peoGroup, permissionSetId,
                Release.EditReleaseStage, PermissionId.Allow).ConfigureAwait(false);
        }

        private async Task UpdatePermissionAsync(string projectId, Response.ApplicationGroup @group,
            Response.PermissionsSetId permissions, int bit, int to)
        {
            var permission = permissions.Permissions.Single(p => p.PermissionBit == bit);
            permission.PermissionId = to;

            await _client.PostAsync(Permissions.ManagePermissions(projectId),
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