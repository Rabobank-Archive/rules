using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules
{
    public class OnlyProjectAdministratorsCanDeleteTheTeamProject : IProjectRule
    {
        private readonly VstsRestClient _client;

        public OnlyProjectAdministratorsCanDeleteTheTeamProject(VstsRestClient client)
        {
            _client = client;
        }


        public string Description => "Only Project Administrators can delete the Team Project";

        public bool Evaluate(string project)
        {
            var permissions = _client.Get(VstsService.Requests.ApplicationGroup.ApplicationGroups(project))
                .Identities.Select(g =>
                    _client.Get(Permissions.PermissionsGroupProjectId(project, g.TeamFoundationId)));

            return permissions
                       .SelectMany(p => p.Security.Permissions)
                       .Count(s => s.DisplayName == "Delete team project" &&
                                   (s.PermissionId == 1 || s.PermissionId == 3)) == 1;
        }
    }
}