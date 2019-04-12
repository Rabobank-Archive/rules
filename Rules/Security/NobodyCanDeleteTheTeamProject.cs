using System;
using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheTeamProject : IProjectRule
    {
        private const string RabobankProjectAdministrators = "Rabobank Project Administrators";
        private const string DeleteTeamProject = "Delete team project";
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

        public void Fix(string project)
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

            ManagePermissionsForAllOtherGroups(project, groups);
            ManageRabobankProjectAdministratorsGroupPermissions(project, raboId);
        }

        private void ManagePermissionsForAllOtherGroups(string project, ApplicationGroups groups)
        {
            foreach (var identity in groups
                .Identities
                .Where(i => i.FriendlyDisplayName != "Project Administrators")
                .Where(i => i.FriendlyDisplayName != RabobankProjectAdministrators))
            {
                var permissions =
                    _client.Get(Permissions.PermissionsGroupProjectId(project, identity.TeamFoundationId));

                var delete = permissions.Security.Permissions.Single(p => p.DisplayName == DeleteTeamProject);
                delete.PermissionBit = PermissionId.NotSet;

                _client.Post(Permissions.ManagePermissions(project,
                    new Permissions.ManagePermissionsData(
                        identity.TeamFoundationId,
                        permissions.Security.DescriptorIdentifier,
                        permissions.Security.DescriptorIdentityType,
                        delete)));
            }
        }

        private void ManageRabobankProjectAdministratorsGroupPermissions(string project, string tfsId)
        {
            var permissions = _client.Get(Permissions.PermissionsGroupProjectId(project, tfsId));
            var delete = permissions.Security.Permissions.Single(p => p.DisplayName == DeleteTeamProject);
            delete.PermissionBit = PermissionId.Deny;

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