using System.Threading.Tasks;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Permissions
{
    internal class ForTeamProject : IFor
    {
        private readonly IVstsRestClient _client;
        private readonly string _project;

        public ForTeamProject(IVstsRestClient client, string project)
        {
            _client = client;
            _project = project;
        }

        public Task<VstsService.Response.ApplicationGroups> IdentitiesAsync() =>
            _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(_project));

        public async Task<VstsService.Response.PermissionsSetId> PermissionSetAsync(VstsService.Response.ApplicationGroup identity) =>
            (await _client.GetAsync(VstsService.Requests.Permissions.PermissionsGroupProjectId(_project, identity.TeamFoundationId)).ConfigureAwait(false)).Security;

        public Task UpdateAsync(VstsService.Response.ApplicationGroup identity, VstsService.Response.PermissionsSetId permissionSet,
            VstsService.Response.Permission permission) =>
            _client.PostAsync(VstsService.Requests.Permissions.ManagePermissions(_project),
                new VstsService.Requests.Permissions.ManagePermissionsData(
                    identity.TeamFoundationId,
                    permissionSet.DescriptorIdentifier,
                    permissionSet.DescriptorIdentityType,
                    permission).Wrap());
    }
}