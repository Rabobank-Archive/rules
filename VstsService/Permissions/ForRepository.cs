using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.VstsService.Permissions
{
    public class ForRepository : IFor
    {
        private readonly IVstsRestClient _client;
        private readonly string _projectId;
        private readonly string _itemId;

        public ForRepository(IVstsRestClient client, string projectId, string itemId)
        {
            _client = client;
            _projectId = projectId;
            _itemId = itemId;
        }

        public Task<Response.ApplicationGroups> IdentitiesAsync() =>
            _client.GetAsync(ApplicationGroup.ExplicitIdentitiesRepos(_projectId, SecurityNamespaceIds.GitRepositories, _itemId));

        public Task<Response.PermissionsSetId> PermissionSetAsync(Response.ApplicationGroup identity) =>
            _client.GetAsync(Requests.Permissions.PermissionsGroupRepository(_projectId, SecurityNamespaceIds.GitRepositories,identity.TeamFoundationId, _itemId));

        public Task UpdateAsync(Response.ApplicationGroup identity,
                Response.PermissionsSetId permissionSet, Response.Permission permission) =>
            _client.PostAsync(Requests.Permissions.ManagePermissions(_projectId),
                new ManagePermissionsData(identity.TeamFoundationId, permissionSet.DescriptorIdentifier, 
                permissionSet.DescriptorIdentityType, permission.PermissionToken, permission).Wrap());
    }
}