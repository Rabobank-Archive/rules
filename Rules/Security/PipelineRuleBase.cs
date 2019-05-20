using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override IEnumerable<ApplicationGroup> LoadGroups(string projectId, string id) =>
            _client.Get(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, NamespaceId, id)).Identities;

        protected override PermissionsSetId LoadPermissionsSetForGroup(string projectId, string id, ApplicationGroup group) =>
            _client.Get(Permissions.PermissionsGroupSetIdDefinition(projectId, NamespaceId, group.TeamFoundationId, id));

        protected override void UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission) =>
            _client.Post(Permissions.ManagePermissions(projectId, new Permissions.ManagePermissionsData(@group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission)));

        protected abstract string NamespaceId { get; } // https://dev.azure.com/somecompany/_apis/securitynamespaces?api-version=5.0
    }
}