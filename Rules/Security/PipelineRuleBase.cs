using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
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

        protected override async Task<PermissionsSetId> LoadPermissionsSetForGroup(string projectId, string id,
            ApplicationGroup @group) =>
            await _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(projectId, NamespaceId, group.TeamFoundationId, id));

        protected override async void UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission) =>
            await _client.PostAsync(Permissions.ManagePermissions(projectId), new Permissions.ManagePermissionsData(@group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission).Wrap());

        protected abstract string NamespaceId { get; } // https://dev.azure.com/somecompany/_apis/securitynamespaces?api-version=5.0
    }
}