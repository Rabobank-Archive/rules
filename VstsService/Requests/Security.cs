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

        public static IVstsPostRequest<AddMemberData, object> AddMember(string project) => 
            new VstsPostRequest<AddMemberData, object>($"/{project}/_api/_identity/AddIdentities?__v=5");

        public static IVstsPostRequest<EditMembersData, object> EditMembership(string project) =>
            new VstsPostRequest<EditMembersData, object>($"/{project}/_api/_identity/EditMembership?__v=5");

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

        public static IVstsPostRequest<ManageGroupData, Response.ApplicationGroup> ManageGroup(string project) =>
            new VstsPostRequest<ManageGroupData, Response.ApplicationGroup>($"/{project}/_api/_identity/ManageGroup?__v=5");

        public class ManageGroupData
        {
            public string Name { get; set; }
        }

        public static IVstsPostRequest<string, object> DeleteIdentity(string project) =>
            new VstsPostRequest<string, object>($"/{project}/_api/_identity/DeleteIdentity?__v=5");
    }
}