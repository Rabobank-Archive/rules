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
        private const string RabobankProjectAdministrators = "Rabobank Project Administrators";
        private const string DeleteTeamProject = "Delete team project";
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteTheTeamProject(IVstsRestClient client)
        {
            _client = client;
        }

        string IProjectRule.Description => "Nobody can delete the Team Project";

        string IProjectRule.Why => "To enforce auditability, no data should be deleted. Therefore, nobody should be able to delete the Team Project.";

        public async Task<bool> Evaluate(string project)
        {
            var groups = await _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(project));
            return await CheckOnlyProjectAdministratorsHasPermissionToDeleteTeamProject(project, groups) &&
                   await CheckProjectAdministratorsGroupOnlyContainsRabobankAdministrators(project, groups);
        }

        private async Task<bool> CheckOnlyProjectAdministratorsHasPermissionToDeleteTeamProject(string project, Response.ApplicationGroups groups)
        {
            var permissions =  await Task.WhenAll(groups
                .Identities
                .Where(g => g.FriendlyDisplayName != "Project Administrators")
                .Select(async g => await _client.GetAsync(Permissions.PermissionsGroupProjectId(project, g.TeamFoundationId))));

            return !permissions
                       .SelectMany(p => p.Security.Permissions)
                       .Any(s => s.DisplayName == DeleteTeamProject &&
                                   (s.PermissionId == PermissionId.Allow || s.PermissionId == PermissionId.AllowInherited));
        }

        private async Task<bool> CheckProjectAdministratorsGroupOnlyContainsRabobankAdministrators(string project, Response.ApplicationGroups groups)
        {
            var id = groups.Identities.Single(p => p.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            var members = (await _client.GetAsync(VstsService.Requests.ApplicationGroup.GroupMembers(project, id))).Identities;

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

        public async Task Reconcile(string project)
        {
            var groups = await _client.GetAsync(VstsService.Requests.ApplicationGroup.ApplicationGroups(project));
            var paId = groups.Identities.Single(p => p.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            var raboId = (await CreateRabobankProjectAdministratorsGroupsIfNotExists(project, groups)).TeamFoundationId;
            
            var members = (await _client
                .GetAsync(VstsService.Requests.ApplicationGroup.GroupMembers(project, paId)))
                .Identities
                .Where(x => x.TeamFoundationId != raboId)
                .ToList();
                            
            await RemoveAllOtherMembersFromProjectAdministrators(project, members, paId);
            await AddAllMembersToRabobankProjectAdministratorsGroup(project, members, raboId);
            await AddRabobankProjectAdministratorsToProjectAdministratorsGroup(project, raboId, paId);

            await UpdatePermissionToDeleteTeamProjectToNotSet(project, groups);
            await UpdatePermissionToDeleteTeamProjectToDeny(project, raboId);
        }

        private async Task UpdatePermissionToDeleteTeamProjectToNotSet(string project, Response.ApplicationGroups groups)
        {
            foreach (var identity in groups
                .Identities
                .Where(i => i.FriendlyDisplayName != "Project Administrators")
                .Where(i => i.FriendlyDisplayName != RabobankProjectAdministrators))
            {
                var permissions =
                    await _client.GetAsync(Permissions.PermissionsGroupProjectId(project, identity.TeamFoundationId));

                var delete = permissions.Security.Permissions.Single(p => p.DisplayName == DeleteTeamProject);
                delete.PermissionId = PermissionId.NotSet;

                await _client.PostAsync(Permissions.ManagePermissions(project),
                    new Permissions.ManagePermissionsData(
                        identity.TeamFoundationId,
                        permissions.Security.DescriptorIdentifier,
                        permissions.Security.DescriptorIdentityType,
                        delete).Wrap());
            }
        }

        private async Task UpdatePermissionToDeleteTeamProjectToDeny(string project, string tfsId)
        {
            var permissions = await _client.GetAsync(Permissions.PermissionsGroupProjectId(project, tfsId));
            var delete = permissions.Security.Permissions.Single(p => p.DisplayName == DeleteTeamProject);
            delete.PermissionId = 2;
            delete.PermissionBit = 4;

            await _client.PostAsync(Permissions.ManagePermissions(project),
                new Permissions.ManagePermissionsData(
                    tfsId,
                    permissions.Security.DescriptorIdentifier,
                    permissions.Security.DescriptorIdentityType,
                    delete).Wrap());
        }

        private async Task<ApplicationGroup> CreateRabobankProjectAdministratorsGroupsIfNotExists(string project, Response.ApplicationGroups groups)
        {
            return groups.Identities.SingleOrDefault(p => p.FriendlyDisplayName == RabobankProjectAdministrators) ??
                   await _client.PostAsync(VstsService.Requests.Security.ManageGroup(project),
                       new VstsService.Requests.Security.ManageGroupData
                       {
                           Name = RabobankProjectAdministrators
                       });
        }

        private async Task AddAllMembersToRabobankProjectAdministratorsGroup(string project, IEnumerable<ApplicationGroup> members, string rabo)
        {
            await _client.PostAsync(VstsService.Requests.Security.AddMember(project),
                new VstsService.Requests.Security.AddMemberData(
                    members.Select(m => m.TeamFoundationId),
                    new[] {rabo}));
        }

        private async Task RemoveAllOtherMembersFromProjectAdministrators(string project, IEnumerable<ApplicationGroup> members, string id)
        {
            await _client.PostAsync(VstsService.Requests.Security.EditMembership(project),
                new VstsService.Requests.Security.RemoveMembersData(members.Select(m => m.TeamFoundationId), id));
        }

        private async Task AddRabobankProjectAdministratorsToProjectAdministratorsGroup(string project, string rabo, string id)
        {
            await _client.PostAsync(VstsService.Requests.Security.AddMember(project),
                new VstsService.Requests.Security.AddMemberData(new[] {rabo}, new[] {id}));
        }
    }
}