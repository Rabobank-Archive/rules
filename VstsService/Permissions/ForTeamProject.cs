using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService.Permissions
{
    public class ForTeamProject : IFor 
    {
        private readonly IVstsRestClient _client;
        private readonly string _projectId;

        public ForTeamProject(IVstsRestClient client, string projectId)
        {
            _client = client;
            _projectId = projectId;
        }

        public Task<Response.ApplicationGroups> IdentitiesAsync() =>
            _client.GetAsync(Requests.ApplicationGroup.ApplicationGroups(_projectId));

        public async Task<Response.PermissionsSetId> PermissionSetAsync(Response.ApplicationGroup identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            return (await _client.GetAsync(Requests.Permissions.PermissionsGroupProjectId(_projectId, identity.TeamFoundationId)).ConfigureAwait(false)).Security;
        }

        public Task UpdateAsync(Response.ApplicationGroup identity,
                Response.PermissionsSetId permissionSet, Response.Permission permission)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            return _client.PostAsync(Requests.Permissions.ManagePermissions(_projectId),
                new ManagePermissionsData(identity.TeamFoundationId, permissionSet.DescriptorIdentifier,
                permissionSet.DescriptorIdentityType, ExtractToken(permission.PermissionToken), permission).Wrap());
        }

        private static string ExtractToken(string token) => 
            Regex.Match(token, @"^(?:\$PROJECT:)?(.*?)(?::)?$").Groups[1].Value;
    }
}