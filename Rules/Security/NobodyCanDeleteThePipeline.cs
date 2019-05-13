using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteThePipeline : NobodyCanDeleteThisBase, IRule, IReconcile
    {
        private readonly IVstsRestClient _client;
        private readonly string _namespaceId;

        private NobodyCanDeleteThePipeline(IVstsRestClient client, string namespaceId, string permissionsDisplayName)
        {
            _client = client;
            _namespaceId = namespaceId;
            PermissionsDisplayName = permissionsDisplayName;
        }

        protected override string PermissionsDisplayName { get; }
        protected override IEnumerable<int> AllowedPermissions => new[] { PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited };
        protected override IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators",
            "Project Collection Build Administrators",
            "Project Collection Service Accounts"
        };

        string IRule.Description => "Nobody can delete the pipeline";
        string IRule.Why => "To ensure auditability, no data should be deleted. Therefore, nobody should be able to delete the pipeline.";
        string[] IReconcile.Impact => new[]
        {
            "For all application groups the 'Delete Pipeline' permission is set to Deny",
            "For all single users the 'Delete Pipeline' permission is set to Deny"
        };

        protected override IEnumerable<ApplicationGroup> LoadGroups(string projectId, string id) =>
            _client.Get(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, _namespaceId, id)).Identities;

        protected override PermissionsSetId LoadPermissionsSetForGroup(string projectId, string id, ApplicationGroup group) =>
            _client.Get(Permissions.PermissionsGroupSetIdDefinition(projectId, _namespaceId, group.TeamFoundationId, id));

        protected override void UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission) =>
            _client.Post(Permissions.ManagePermissions(projectId, new Permissions.ManagePermissionsData(@group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission)));

        public static IRule Build(IVstsRestClient client) =>
            new NobodyCanDeleteThePipeline(client, GetNameSpaceId("Build", client), "Delete builds");

        public static IRule BuildPipeline(IVstsRestClient client) =>
            new NobodyCanDeleteThePipeline(client, GetNameSpaceId("Build", client), "Delete build definition");

        public static IRule Release(IVstsRestClient client) =>
            new NobodyCanDeleteThePipeline(client, GetNameSpaceId("ReleaseManagement", client), "Delete releases");

        public static IRule ReleasePipeline(IVstsRestClient client) =>
            new NobodyCanDeleteThePipeline(client, GetNameSpaceId("ReleaseManagement", client), "Delete release pipeline");

        private static string GetNameSpaceId(string namespaceName, IVstsRestClient client) =>  
            client
                .Get(VstsService.Requests.SecurityNamespace.SecurityNamespaces())
                .First(s => s.Name == namespaceName)
                .NamespaceId
                .ToString();
    }
}