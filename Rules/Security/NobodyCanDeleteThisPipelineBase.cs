using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class NobodyCanDeleteThisPipelineBase : NobodyCanDeleteThisBase
    {
        private readonly IVstsRestClient _client;
        protected abstract string NamespaceName { get; }

        public NobodyCanDeleteThisPipelineBase(IVstsRestClient client)
        {
            _client = client;
        }

        protected override IEnumerable<ApplicationGroup> LoadGroups(string projectId, string id) =>
            _client.Get(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, GetNamespaceId(NamespaceName), id)).Identities;

        protected override PermissionsSetId LoadPermissionsSetForGroup(string projectId, string id, ApplicationGroup group) =>
            _client.Get(Permissions.PermissionsGroupSetIdDefinition(projectId, GetNamespaceId(NamespaceName), group.TeamFoundationId, id));

        protected override void UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission) =>
            _client.Post(Permissions.ManagePermissions(projectId, new Permissions.ManagePermissionsData(@group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission)));

        public string GetNamespaceId(string namespaceName)
        {
            var namespaces = _client.Get(VstsService.Requests.SecurityNamespace.SecurityNamespaces());
            return (namespaces.Any(s => s.Name == namespaceName) ?
                namespaces.First(s => s.Name == namespaceName).NamespaceId :
                namespaces.SelectMany(x => x.Actions).First(s => s.Name == namespaceName).NamespaceId.ToString());
        }
    }
}