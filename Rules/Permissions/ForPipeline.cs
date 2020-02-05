using System;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Permissions
{
    internal class ForPipeline : IFor
    {
        private readonly IVstsRestClient _client;
        private readonly string _projectId;
        private readonly string _namespaceId;
        private readonly string _itemId;
        private readonly string _itemPath;

        public ForPipeline(IVstsRestClient client, string projectId, string namespaceId, string itemId, string itemPath)
        {
            _client = client;
            _projectId = projectId;
            _namespaceId = namespaceId;
            _itemId = itemId;
            _itemPath = itemPath;
        }
        
        public Task<VstsService.Response.ApplicationGroups> IdentitiesAsync() => 
            _client.GetAsync(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(_projectId, _namespaceId, _itemId));
        
        public Task<VstsService.Response.PermissionsSetId> PermissionSetAsync(VstsService.Response.ApplicationGroup identity) =>
            _client.GetAsync(VstsService.Requests.Permissions.PermissionsGroupSetIdDefinition(
            _projectId, _namespaceId,identity.TeamFoundationId, ExtractToken()));

        public Task UpdateAsync(VstsService.Response.ApplicationGroup identity,
                VstsService.Response.PermissionsSetId permissionSet, VstsService.Response.Permission permission) =>
            _client.PostAsync(VstsService.Requests.Permissions.ManagePermissions(_projectId),
                new ManagePermissionsData(identity.TeamFoundationId, permissionSet.DescriptorIdentifier, 
                permissionSet.DescriptorIdentityType, permission.PermissionToken, permission).Wrap());

        private string ExtractToken() => 
            _itemPath == "\\"
                ? $"{_projectId}/{_itemId}"
                : $"{_projectId}{_itemPath.Replace("\\", "/")}/{_itemId}";
    }
}