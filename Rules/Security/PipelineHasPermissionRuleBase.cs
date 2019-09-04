using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using System;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class PipelineHasPermissionRuleBase : RuleBase
    {
        readonly IVstsRestClient _client;

        protected PipelineHasPermissionRuleBase(IVstsRestClient client)
        {
            _client = client;
        }

        protected override async Task<IEnumerable<ApplicationGroup>> LoadGroupsAsync(string projectId, string id) =>
            (await _client.GetAsync(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, NamespaceId, id))
                .ConfigureAwait(false))
            .Identities;

        protected async Task<IEnumerable<ApplicationGroup>> LoadGroupsAsync(string projectId) =>
            (await _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(projectId))
                .ConfigureAwait(false))
            .Identities;

        protected override Task<Response.PermissionsSetId> LoadPermissionsSetForGroupAsync(string projectId, string id,
            ApplicationGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return LoadPermissionsSetForGroupInternalAsync(projectId, id, group);
        }

        private async Task<Response.PermissionsSetId> LoadPermissionsSetForGroupInternalAsync(string projectId, string id, ApplicationGroup group)
        {
            return await _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(projectId, NamespaceId, group.TeamFoundationId, id))
                            .ConfigureAwait(false);
        }

        protected Task<Response.PermissionsSetId> LoadPermissionsSetForGroupAsync(string projectId, ApplicationGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return LoadPermissionsSetForGroupInternalAsync(projectId, group);
        }

        private async Task<Response.PermissionsSetId> LoadPermissionsSetForGroupInternalAsync(string projectId, ApplicationGroup group)
        {
            return await _client.GetAsync(Permissions.PermissionsGroupSetId(projectId, NamespaceId, group.TeamFoundationId))
                            .ConfigureAwait(false);
        }

        protected override Task UpdatePermissionAsync(string projectId, ApplicationGroup group,
            Response.PermissionsSetId permissionSetId, Response.Permission permission)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (permissionSetId == null)
                throw new ArgumentNullException(nameof(permissionSetId));

            return UpdatePermissionInternalAsync(projectId, group, permissionSetId, permission);
        }

        private async Task UpdatePermissionInternalAsync(string projectId, ApplicationGroup group, Response.PermissionsSetId permissionSetId, Response.Permission permission)
        {
            await _client.PostAsync(Permissions.ManagePermissions(projectId), new Permissions.ManagePermissionsData(
                            group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission)
                            .Wrap())
                            .ConfigureAwait(false);
        }

        protected async Task<ApplicationGroup> CreateProductionEnvironmentOwnersGroupAsync(string projectId) =>
            await _client.PostAsync(VstsService.Requests.Security.ManageGroup(projectId),
                new VstsService.Requests.Security.ManageGroupData
                {
                    Name = "Production Environment Owners"
                })
            .ConfigureAwait(false);
    }
}