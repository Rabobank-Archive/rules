using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Permissions
{
    internal class ForTeamProject : IFor 
    {
        private readonly IVstsRestClient _client;
        private readonly string _projectId;

        public ForTeamProject(IVstsRestClient client, string projectId)
        {
            _client = client;
            _projectId = projectId;
        }

        public Task<VstsService.Response.ApplicationGroups> IdentitiesAsync() =>
            _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(_projectId));

        public async Task<VstsService.Response.PermissionsSetId> PermissionSetAsync(VstsService.Response.ApplicationGroup identity) =>
            (await _client.GetAsync(VstsService.Requests.Permissions.PermissionsGroupProjectId(_projectId, identity.TeamFoundationId)).ConfigureAwait(false)).Security;

        public Task UpdateAsync(VstsService.Response.ApplicationGroup identity,
                VstsService.Response.PermissionsSetId permissionSet, VstsService.Response.Permission permission) =>
            _client.PostAsync(VstsService.Requests.Permissions.ManagePermissions(_projectId),
                new VstsService.Requests.Permissions.ManagePermissionsData(identity.TeamFoundationId,
                permissionSet.DescriptorIdentifier, permissionSet.DescriptorIdentityType,
                ExtractToken(permission.PermissionToken), permission).Wrap());

        private static string ExtractToken(string token) => 
            Regex.Match(token, @"^(?:\$PROJECT:)?(.*?)(?::)?$").Groups[1].Value;
    }
}