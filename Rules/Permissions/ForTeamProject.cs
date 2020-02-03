using System.Threading.Tasks;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Permissions
{
    internal class ForTeamProject : ForAll, IFor 
    {
        private readonly IVstsRestClient _client;
        private readonly string _projectId;

        public ForTeamProject(IVstsRestClient client, string projectId) : base(client, projectId)
        {
            _client = client;
            _projectId = projectId;
        }

        public Task<VstsService.Response.ApplicationGroups> IdentitiesAsync() =>
            _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(_projectId));

        public async Task<VstsService.Response.PermissionsSetId> PermissionSetAsync(VstsService.Response.ApplicationGroup identity) =>
            (await _client.GetAsync(VstsService.Requests.Permissions.PermissionsGroupProjectId(_projectId, identity.TeamFoundationId)).ConfigureAwait(false)).Security;
    }
}