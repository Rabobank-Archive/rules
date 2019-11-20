using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.Rules.Permissions;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheTeamProject : IProjectRule, IProjectReconcile
    {
        private const int DeleteProjectPermissionBit = 4;
        private const string RabobankProjectAdministrators = "Rabobank Project Administrators";
        private const string ProjectAdministrators = "Project Administrators";
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteTheTeamProject(IVstsRestClient client) => _client = client;

        public string Description => "Nobody can delete the Team Project (SOx)";
        public string Link => "https://confluence.dev.somecompany.nl/x/NY8AD";
        public bool IsSox => true;

        public async Task<bool> EvaluateAsync(string projectId)
        {
            return await Permissions(projectId)
                       .ValidateAsync()
                       .ConfigureAwait(false)
                   && await PermissionsForRaboProjectAdministrators(projectId)
                       .ValidateAsync()
                       .ConfigureAwait(false)
                   && await CheckProjectAdministratorsGroupOnlyContainsRabobankAdministratorsAsync(projectId)
                       .ConfigureAwait(false);
        }
        
        private async Task<bool> CheckProjectAdministratorsGroupOnlyContainsRabobankAdministratorsAsync(string project)
        {
            var groups = await _client
                .GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(project))
                .ConfigureAwait(false);
            
            var id = groups
                .Identities
                .Single(p => p.FriendlyDisplayName == ProjectAdministrators)
                .TeamFoundationId;
            
            var members = (await _client
                    .GetAsync(VstsService.Requests.ApplicationGroup.GroupMembers(project, id))
                    .ConfigureAwait(false))
                .Identities;

            return members.All(m => m.FriendlyDisplayName == RabobankProjectAdministrators);
        }

        string[] IProjectReconcile.Impact => new[]
        {
            "Rabobank Project Administrators group is created and added to Project Administrators",
            "Delete team project permissions of the Rabobank Project Administrators group is set to deny",
            "Members of the Project Administrators are moved to Rabobank Project Administrators",
            "Delete team project permission is set to 'not set' for all other groups"
        };

        public async Task ReconcileAsync(string projectId)
        {
            var groups = await _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(projectId)).ConfigureAwait(false);
            var paId = groups.Identities.Single(p => p.FriendlyDisplayName == ProjectAdministrators).TeamFoundationId;
            var raboId = (await CreateRabobankProjectAdministratorsGroupsIfNotExistsAsync(projectId, groups).ConfigureAwait(false))
                .TeamFoundationId;

            var members = (await _client.GetAsync(VstsService.Requests.ApplicationGroup.GroupMembers(projectId, paId)).ConfigureAwait(false))
                .Identities
                .Where(x => x.TeamFoundationId != raboId)
                .ToList();

            await RemoveAllOtherMembersFromProjectAdministratorsAsync(projectId, members, paId).ConfigureAwait(false);
            await AddAllMembersToRabobankProjectAdministratorsGroupAsync(projectId, members, raboId).ConfigureAwait(false);
            await AddRabobankProjectAdministratorsToProjectAdministratorsGroupAsync(projectId, raboId, paId).ConfigureAwait(false);

            await Permissions(projectId)
                .SetToAsync(PermissionId.NotSet).ConfigureAwait(false);
            await PermissionsForRaboProjectAdministrators(projectId)
                .SetToAsync(PermissionId.Deny).ConfigureAwait(false);
        }

        private ManagePermissions Permissions(string project) =>
            ManagePermissions
                .ForTeamProject(_client, project)
                .Ignore(ProjectAdministrators, RabobankProjectAdministrators)
                .Permissions((DeleteProjectPermissionBit, SecurityNamespaceIds.Project))
                .Allow(PermissionId.Deny, PermissionId.DenyInherited, PermissionId.NotSet);

        private ManagePermissions PermissionsForRaboProjectAdministrators(string project) =>
            ManagePermissions
                .ForTeamProject(_client, project)
                .For(RabobankProjectAdministrators)
                .Permissions((DeleteProjectPermissionBit, SecurityNamespaceIds.Project))
                .Allow(PermissionId.Deny, PermissionId.DenyInherited);
        
        private async Task<ApplicationGroup> CreateRabobankProjectAdministratorsGroupsIfNotExistsAsync(string project, Response.ApplicationGroups groups) =>
            groups.Identities.SingleOrDefault(p => p.FriendlyDisplayName == RabobankProjectAdministrators) 
            ?? await _client
                .PostAsync(VstsService.Requests.Security.ManageGroup(project), new VstsService.Requests.Security.ManageGroupData { Name = RabobankProjectAdministrators })
                .ConfigureAwait(false);

        private async Task AddAllMembersToRabobankProjectAdministratorsGroupAsync(string project, IEnumerable<ApplicationGroup> members, string rabo) =>
            await _client.PostAsync(VstsService.Requests.Security.AddMember(project),
                    new VstsService.Requests.Security.AddMemberData(
                        members.Select(m => m.TeamFoundationId),
                        new[] { rabo }))
                .ConfigureAwait(false);

        private async Task RemoveAllOtherMembersFromProjectAdministratorsAsync(string project, IEnumerable<ApplicationGroup> members, string id) =>
            await _client.PostAsync(VstsService.Requests.Security.EditMembership(project),
                    new VstsService.Requests.Security.RemoveMembersData(members.Select(m => m.TeamFoundationId), id))
                .ConfigureAwait(false);

        private async Task AddRabobankProjectAdministratorsToProjectAdministratorsGroupAsync(string project, string rabo, string id) =>
            await _client.PostAsync(VstsService.Requests.Security.AddMember(project),
                    new VstsService.Requests.Security.AddMemberData(new[] { rabo }, new[] { id }))
                .ConfigureAwait(false);
    }
}