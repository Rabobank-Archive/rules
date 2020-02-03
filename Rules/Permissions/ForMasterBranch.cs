using System;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Permissions
{
    internal class ForMasterBranch : IFor
    {
        private readonly IVstsRestClient _client;
        private readonly string _projectId;
        private readonly string _itemId;

        public ForMasterBranch(IVstsRestClient client, string projectId, string itemId)
        {
            _client = client;
            _projectId = projectId;
            _itemId = itemId;
        }

        public Task<VstsService.Response.ApplicationGroups> IdentitiesAsync() =>
            _client.GetAsync(ApplicationGroup.ExplicitIdentitiesMasterBranch(_projectId, SecurityNamespaceIds.GitRepositories, _itemId));
        
        public Task<VstsService.Response.PermissionsSetId> PermissionSetAsync(VstsService.Response.ApplicationGroup identity) =>
            _client.GetAsync(VstsService.Requests.Permissions.PermissionsGroupMasterBranch(_projectId, SecurityNamespaceIds.GitRepositories, identity.TeamFoundationId, _itemId));

        public Task UpdateAsync(VstsService.Response.ApplicationGroup identity,
                VstsService.Response.PermissionsSetId permissionSet, VstsService.Response.Permission permission) =>
            _client.PostAsync(VstsService.Requests.Permissions.ManagePermissions(_projectId),
                new VstsService.Requests.Permissions.ManagePermissionsData(identity.TeamFoundationId,
                permissionSet.DescriptorIdentifier, permissionSet.DescriptorIdentityType, 
                ExtractToken(permission.PermissionToken), permission).Wrap());

        private static string ExtractToken(string token) => 
            token.Replace("6d0061007300740065007200", "master", StringComparison.CurrentCulture);
    }
}