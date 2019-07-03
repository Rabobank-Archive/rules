using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class PipelineRuleBase : RuleBase
    {
        readonly IVstsRestClient _client;

        protected PipelineRuleBase(IVstsRestClient client)
        {
            _client = client;
        }

        protected override async Task<IEnumerable<ApplicationGroup>> LoadGroups(string projectId, string id) =>
            (await _client.GetAsync(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, NamespaceId, id))).Identities;

        protected async Task<IEnumerable<ApplicationGroup>> LoadGroups(string projectId) =>
            (await _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(projectId))).Identities;

        protected override async Task<Response.PermissionsSetId> LoadPermissionsSetForGroup(string projectId, string id,
            ApplicationGroup group) =>
            await _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(projectId, NamespaceId, group.TeamFoundationId, id));

        protected async Task<Response.PermissionsSetId> LoadPermissionsSetForGroup(string projectId, ApplicationGroup group) =>
            await _client.GetAsync(Permissions.PermissionsGroupSetId(projectId, NamespaceId, group.TeamFoundationId));

        protected override async Task UpdatePermission(string projectId, ApplicationGroup group, Response.PermissionsSetId permissionSetId, Response.Permission permission) =>
            await _client.PostAsync(Permissions.ManagePermissions(projectId), new Permissions.ManagePermissionsData(group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission).Wrap());

        protected async Task<ApplicationGroup> CreateProductionEnvironmentOwnersGroup(string projectId) =>
            await _client.PostAsync(VstsService.Requests.Security.ManageGroup(projectId),
                new VstsService.Requests.Security.ManageGroupData
                {
                    Name = "Production Environment Owners"
                });
    }
}