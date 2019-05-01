using System;
using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
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
                       .Any(s => s.DisplayName == DeleteTeamProject &&
                                   (s.PermissionId == PermissionId.Allow || s.PermissionId == PermissionId.AllowInherited));
        }

        private bool CheckProjectAdministratorsGroupOnlyContainsRabobankAdministrators(string project, ApplicationGroups groups)
        {
            var id = groups.Identities.Single(p => p.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            var members = _client.Get(VstsService.Requests.ApplicationGroup.GroupMembers(project, id)).Identities;

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

        public void Reconcile(string project)
        {
            var groups = _client.Get(VstsService.Requests.ApplicationGroup.ApplicationGroups(project));
            var paId = groups.Identities.Single(p => p.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            var raboId = CreateRabobankProjectAdministratorsGroupsIfNotExists(project, groups).TeamFoundationId;
            
            var members = _client
                .Get(VstsService.Requests.ApplicationGroup.GroupMembers(project, paId))
                .Identities
                .Where(x => x.TeamFoundationId != raboId);
                            
            RemoveAllOtherMembersFromProjectAdministrators(project, members, paId);
            AddAllMembersToRabobankProjectAdministratorsGroup(project, members, raboId);
            AddRabobankProjectAdministratorsToProjectAdministratorsGroup(project, raboId, paId);

            UpdatePermissionToDeleteTeamProjectToNotSet(project, groups);
            UpdatePermissionToDeleteTeamProjectToDeny(project, raboId);
        }

        private void UpdatePermissionToDeleteTeamProjectToNotSet(string project, ApplicationGroups groups)
        {
            foreach (var identity in groups
                .Identities
                .Where(i => i.FriendlyDisplayName != "Project Administrators")
                .Where(i => i.FriendlyDisplayName != RabobankProjectAdministrators))
            {
                var permissions =
                    _client.Get(Permissions.PermissionsGroupProjectId(project, identity.TeamFoundationId));

                var delete = permissions.Security.Permissions.Single(p => p.DisplayName == DeleteTeamProject);
                delete.PermissionId = PermissionId.NotSet;

                _client.Post(Permissions.ManagePermissions(project,
                    new Permissions.ManagePermissionsData(
                        identity.TeamFoundationId,
                        permissions.Security.DescriptorIdentifier,
                        permissions.Security.DescriptorIdentityType,
                        delete)));
            }
        }

        private void UpdatePermissionToDeleteTeamProjectToDeny(string project, string tfsId)
        {
            var permissions = _client.Get(Permissions.PermissionsGroupProjectId(project, tfsId));
            var delete = permissions.Security.Permissions.Single(p => p.DisplayName == DeleteTeamProject);
            delete.PermissionId = 2;
            delete.PermissionBit = 4;

            _client.Post(Permissions.ManagePermissions(project,
                new Permissions.ManagePermissionsData(
                    tfsId,
                    permissions.Security.DescriptorIdentifier,
                    permissions.Security.DescriptorIdentityType,
                    delete)));
        }

        private ApplicationGroup CreateRabobankProjectAdministratorsGroupsIfNotExists(string project, ApplicationGroups groups)
        {
            return groups.Identities.SingleOrDefault(p => p.FriendlyDisplayName == RabobankProjectAdministrators) ??
                   _client.Post(VstsService.Requests.Security.ManageGroup(project, new VstsService.Requests.Security.ManageGroupData
                   {
                       Name = RabobankProjectAdministrators
                   }));
        }

        private void AddAllMembersToRabobankProjectAdministratorsGroup(string project, IEnumerable<ApplicationGroup> members, string rabo)
        {
            _client.Post(VstsService.Requests.Security.AddMember(project,
                new VstsService.Requests.Security.AddMemberData(
                    members.Select(m => m.TeamFoundationId),
                    new[] {rabo})));
        }

        private void RemoveAllOtherMembersFromProjectAdministrators(string project, IEnumerable<ApplicationGroup> members, string id)
        {
            _client.Post(VstsService.Requests.Security.EditMembership(project,
                new VstsService.Requests.Security.RemoveMembersData(members.Select(m => m.TeamFoundationId), id)));
        }

        private void AddRabobankProjectAdministratorsToProjectAdministratorsGroup(string project, string rabo, string id)
        {
            _client.Post(VstsService.Requests.Security.AddMember(project,
                new VstsService.Requests.Security.AddMemberData(new[] {rabo}, new[] {id})));
        }
    }
}