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
        private readonly IVstsRestClient _client;

        protected PipelineRuleBase(IVstsRestClient client)
        {
            _client = client;
        }

        protected override async Task<IEnumerable<ApplicationGroup>> LoadGroups(string projectId, string id) =>
            (await _client.GetAsync(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, NamespaceId, id))).Identities;

        protected override async Task<Response.PermissionsSetId> LoadPermissionsSetForGroup(string projectId, string id,
            ApplicationGroup @group) =>
            await _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(projectId, NamespaceId, group.TeamFoundationId, id));

        protected override async Task UpdatePermissionToDeny(string projectId, ApplicationGroup group, Response.PermissionsSetId permissionSetId, Response.Permission permission) =>
            await _client.PostAsync(Permissions.ManagePermissions(projectId), new Permissions.ManagePermissionsData(@group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission).Wrap());
    }
}