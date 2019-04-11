using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheTeamProject : IProjectRule
    {
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteTheTeamProject(IVstsRestClient client)
        {
            _client = client;
        }

        public string Description => "Nobody can delete the Team Project";

        public bool Evaluate(string project)
        {
            var groups = _client.Get(VstsService.Requests.ApplicationGroup.ApplicationGroups(project));
            return CheckOnlyProjectAdministratorsHasPermissionToDeleteTeamProject(project, groups) &&
                   CheckProjectAdministratorsGroupOnlyContainsRabobankAdministrators(project, groups);
        }

        private bool CheckOnlyProjectAdministratorsHasPermissionToDeleteTeamProject(string project, ApplicationGroups groups)
        {
            var permissions = groups
                .Identities
                .Where(g => g.FriendlyDisplayName != "Project Administrators")
                .Select(g => _client.Get(Permissions.PermissionsGroupProjectId(project, g.TeamFoundationId)));

            return !permissions
                       .SelectMany(p => p.Security.Permissions)
                       .Any(s => s.DisplayName == "Delete team project" &&
                                   (s.PermissionId == PermissionId.Allow || s.PermissionId == PermissionId.AllowInherited));
        }
        
        private bool CheckProjectAdministratorsGroupOnlyContainsRabobankAdministrators(string project, ApplicationGroups groups)
        {
            var id = groups.Identities.Single(p => p.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            var members = _client.Get(VstsService.Requests.ApplicationGroup.GroupMembers(project, id)).Identities;

            return
                members.All(m => m.FriendlyDisplayName == "Rabobank Project Administrators");
        }
    }
}