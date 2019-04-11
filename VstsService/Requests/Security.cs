using System.Collections.Generic;
using Newtonsoft.Json;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Security
    {
        public static IVstsRestRequest<Response.Security.IdentityGroup> GroupMembers(string projectId, string groupId)
        {
            return new VstsRestRequest<Response.Security.IdentityGroup>($"/{projectId}/_api/_identity/ReadGroupMembers?__v=5&scope={groupId}&readMembers=true");
        }

        public static IVstsRestRequest<Response.Security.IdentityGroup> Groups(string projectId)
        {
            return new VstsRestRequest<Response.Security.IdentityGroup>($"/{projectId}/_api/_identity/ReadScopedApplicationGroupsJson?__v=5");
        }

        public static IVstsPostRequest<object> AddMember(string project, AddMemberData body) => 
            new VstsPostRequest<object>($"/{project}/_api/_identity/AddIdentities?__v=5", body);

        public static IVstsPostRequest<object> EditMembership(string project, EditMembersData data) =>
            new VstsPostRequest<object>($"/{project}/_api/_identity/EditMembership?__v=5", data);

        public class EditMembersData
        {
            protected EditMembersData(string groupId) => GroupId = groupId;

            public bool EditMembers { get; } = true;
            public string GroupId { get; }
        }

        public class RemoveMembersData : EditMembersData
        {
            public RemoveMembersData(IEnumerable<string> users, string group) : base(group)
            {
                RemoveItemsJson = JsonConvert.SerializeObject(users);
            }

            public string RemoveItemsJson { get; }
        }
        
        public class AddMemberData
        {
            public string ExistingUsersJson { get; }
            public string GroupsToJoinJson { get; }

            public AddMemberData(IEnumerable<string> users, IEnumerable<string> groups)
            {
                ExistingUsersJson = JsonConvert.SerializeObject(users);
                GroupsToJoinJson = JsonConvert.SerializeObject(groups);
            }
        }

        public static IVstsPostRequest<Response.ApplicationGroup> ManageGroup(string project, ManageGroupData data) =>
            new VstsPostRequest<Response.ApplicationGroup>($"/{project}/_api/_identity/ManageGroup?__v=5", data);

        public class ManageGroupData
        {
            public string Name { get; set; }
        }

        public static IVstsPostRequest<object> DeleteIdentity(string project, string tfid) =>
            new VstsPostRequest<object>($"/{project}/_api/_identity/DeleteIdentity?__v=5", tfid);
    }
}