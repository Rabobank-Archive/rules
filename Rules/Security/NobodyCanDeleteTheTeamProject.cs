using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheTeamProject : IProjectRule, IProjectReconcile
    {
        private const int DeleteProjectPermissionId = 2;
        private const int DeleteProjectPermissionBit = 4;
        private const string RabobankProjectAdministrators = "Rabobank Project Administrators";
        private const string ProjectAdministrators = "Project Administrators";
        private const string DeleteTeamProject = "Delete team project";
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteTheTeamProject(IVstsRestClient client)
        {
            _client = client;
        }

        public string Description => "Nobody can delete the Team Project";

        public string Why => "To enforce auditability, no data should be deleted. Therefore, nobody should be able to delete the Team Project.";
        public bool IsSox => true;

        public async Task<bool> EvaluateAsync(string project)
        {
            var groups = await _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(project))
                .ConfigureAwait(false);
            return await CheckOnlyProjectAdministratorsHasPermissionToDeleteTeamProjectAsync(project, groups).ConfigureAwait(false) &&
                   await CheckProjectAdministratorsGroupOnlyContainsRabobankAdministratorsAsync(project, groups).ConfigureAwait(false);
        }

        private async Task<bool> CheckOnlyProjectAdministratorsHasPermissionToDeleteTeamProjectAsync(string project, Response.ApplicationGroups groups)
        {
            var permissions =  await Task.WhenAll(groups
                .Identities
                .Where(g => g.FriendlyDisplayName != ProjectAdministrators)
                .Select(async g => await _client.GetAsync(Permissions.PermissionsGroupProjectId(project, g.TeamFoundationId)).ConfigureAwait(false)))
                .ConfigureAwait(false);

            return !permissions
                       .SelectMany(p => p.Security.Permissions)
                       .Any(s => s.DisplayName == DeleteTeamProject &&
                                   (s.PermissionId == PermissionId.Allow || s.PermissionId == PermissionId.AllowInherited));
        }

        private async Task<bool> CheckProjectAdministratorsGroupOnlyContainsRabobankAdministratorsAsync(string project, Response.ApplicationGroups groups)
        {
            var id = groups.Identities.Single(p => p.FriendlyDisplayName == ProjectAdministrators).TeamFoundationId;
            var members = (await _client.GetAsync(VstsService.Requests.ApplicationGroup.GroupMembers(project, id)).ConfigureAwait(false))
                .Identities;

            return
                members.All(m => m.FriendlyDisplayName == RabobankProjectAdministrators);
        }

        string[] IProjectReconcile.Impact => new[]
        {
            "Rabobank Project Administrators group is created and added to Project Administrators", 
            "Delete team project permissions of the Rabobank Project Administrators group is set to deny", 
            "Members of the Project Administrators are moved to Rabobank Project Administrators", 
            "Delete team project permission is set to 'not set' for all other groups"
        };

        public async Task ReconcileAsync(string project)
        {
            var groups = await _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(project)).ConfigureAwait(false);
            var paId = groups.Identities.Single(p => p.FriendlyDisplayName == ProjectAdministrators).TeamFoundationId;
            var raboId = (await CreateRabobankProjectAdministratorsGroupsIfNotExistsAsync(project, groups).ConfigureAwait(false))
                .TeamFoundationId;
            
            var members = (await _client.GetAsync(VstsService.Requests.ApplicationGroup.GroupMembers(project, paId)).ConfigureAwait(false))
                .Identities
                .Where(x => x.TeamFoundationId != raboId)
                .ToList();
                            
            await RemoveAllOtherMembersFromProjectAdministratorsAsync(project, members, paId).ConfigureAwait(false);
            await AddAllMembersToRabobankProjectAdministratorsGroupAsync(project, members, raboId).ConfigureAwait(false);
            await AddRabobankProjectAdministratorsToProjectAdministratorsGroupAsync(project, raboId, paId).ConfigureAwait(false);

            await UpdatePermissionToDeleteTeamProjectToNotSetAsync(project, groups).ConfigureAwait(false);
            await UpdatePermissionToDeleteTeamProjectToDenyAsync(project, raboId).ConfigureAwait(false);
        }

        private async Task UpdatePermissionToDeleteTeamProjectToNotSetAsync(string project, Response.ApplicationGroups groups)
        {
            foreach (var identity in groups
                .Identities
                .Where(i => i.FriendlyDisplayName != ProjectAdministrators)
                .Where(i => i.FriendlyDisplayName != RabobankProjectAdministrators))
            {
                var permissions =
                    await _client.GetAsync(Permissions.PermissionsGroupProjectId(project, identity.TeamFoundationId))
                        .ConfigureAwait(false);

                var delete = permissions.Security.Permissions.Single(p => p.DisplayName == DeleteTeamProject);
                delete.PermissionId = PermissionId.NotSet;

                await _client.PostAsync(Permissions.ManagePermissions(project),
                    new Permissions.ManagePermissionsData(
                        identity.TeamFoundationId,
                        permissions.Security.DescriptorIdentifier,
                        permissions.Security.DescriptorIdentityType,
                        delete).Wrap())
                    .ConfigureAwait(false);
            }
        }

        private async Task UpdatePermissionToDeleteTeamProjectToDenyAsync(string project, string tfsId)
        {
            var permissions = await _client.GetAsync(Permissions.PermissionsGroupProjectId(project, tfsId)).ConfigureAwait(false);
            var delete = permissions.Security.Permissions.Single(p => p.DisplayName == DeleteTeamProject);
            delete.PermissionId = DeleteProjectPermissionId;
            delete.PermissionBit = DeleteProjectPermissionBit;

            await _client.PostAsync(Permissions.ManagePermissions(project),
                new Permissions.ManagePermissionsData(
                    tfsId,
                    permissions.Security.DescriptorIdentifier,
                    permissions.Security.DescriptorIdentityType,
                    delete).Wrap())
                .ConfigureAwait(false);
        }

        private async Task<ApplicationGroup> CreateRabobankProjectAdministratorsGroupsIfNotExistsAsync(string project, Response.ApplicationGroups groups)
        {
            return groups.Identities.SingleOrDefault(p => p.FriendlyDisplayName == RabobankProjectAdministrators) ??
                   await _client.PostAsync(VstsService.Requests.Security.ManageGroup(project),
                       new VstsService.Requests.Security.ManageGroupData
                       {
                           Name = RabobankProjectAdministrators
                       })
                       .ConfigureAwait(false);
        }

        private async Task AddAllMembersToRabobankProjectAdministratorsGroupAsync(string project, IEnumerable<ApplicationGroup> members, string rabo)
        {
            await _client.PostAsync(VstsService.Requests.Security.AddMember(project),
                new VstsService.Requests.Security.AddMemberData(
                    members.Select(m => m.TeamFoundationId),
                    new[] {rabo}))
                .ConfigureAwait(false);
        }

        private async Task RemoveAllOtherMembersFromProjectAdministratorsAsync(string project, IEnumerable<ApplicationGroup> members, string id)
        {
            await _client.PostAsync(VstsService.Requests.Security.EditMembership(project),
                new VstsService.Requests.Security.RemoveMembersData(members.Select(m => m.TeamFoundationId), id))
                .ConfigureAwait(false);
        }

        private async Task AddRabobankProjectAdministratorsToProjectAdministratorsGroupAsync(string project, string rabo, string id)
        {
            await _client.PostAsync(VstsService.Requests.Security.AddMember(project),
                new VstsService.Requests.Security.AddMemberData(new[] {rabo}, new[] {id}))
                .ConfigureAwait(false);
        }

        public Task<bool> EvaluateAsync(string project, string id)
        {
            throw new System.NotSupportedException();
        }
    }
}