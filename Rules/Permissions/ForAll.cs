using System.Threading.Tasks;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Permissions
{
    internal class ForAll
    {
        private readonly IVstsRestClient _client;
        private readonly string _projectId;

        protected ForAll(IVstsRestClient client, string projectId)
        {
            _client = client;
            _projectId = projectId;
        }

        public Task UpdateAsync(VstsService.Response.ApplicationGroup identity, 
                VstsService.Response.PermissionsSetId permissionSet, VstsService.Response.Permission permission) =>
            _client.PostAsync(VstsService.Requests.Permissions.ManagePermissions(_projectId),
                new VstsService.Requests.Permissions.ManagePermissionsData(identity.TeamFoundationId, 
                permissionSet.DescriptorIdentifier, permissionSet.DescriptorIdentityType, permission).Wrap());
    }
}