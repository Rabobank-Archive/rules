using System;
using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService.Permissions
{
    public class ForPipeline : IFor
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
        
        public Task<Response.ApplicationGroups> IdentitiesAsync() => 
            _client.GetAsync(Requests.ApplicationGroup.ExplicitIdentitiesPipelines(_projectId, _namespaceId, _itemId));

        public Task<Response.PermissionsSetId> PermissionSetAsync(Response.ApplicationGroup identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            return _client.GetAsync(Requests.Permissions.PermissionsGroupSetIdDefinition(
                _projectId, _namespaceId, identity.TeamFoundationId, ExtractToken()));
        }
        public Task UpdateAsync(Response.ApplicationGroup identity,
                Response.PermissionsSetId permissionSet, Response.Permission permission)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            
            return _client.PostAsync(Requests.Permissions.ManagePermissions(_projectId),
                new ManagePermissionsData(identity.TeamFoundationId, permissionSet.DescriptorIdentifier,
                permissionSet.DescriptorIdentityType, permission.PermissionToken, permission).Wrap());
        }

        private string ExtractToken() => 
            _itemPath == "\\"
                ? $"{_projectId}/{_itemId}"
                : $"{_projectId}{_itemPath.Replace("\\", "/", StringComparison.InvariantCulture)}/{_itemId}";
    }
}