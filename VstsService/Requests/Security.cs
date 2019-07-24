using System.Collections.Generic;
using Newtonsoft.Json;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Security
    {
        public static IVstsRequest<Response.Security.IdentityGroup> GroupMembers(string projectId, string groupId) =>
            new VstsRequest<Response.Security.IdentityGroup>($"/{projectId}/_api/_identity/ReadGroupMembers",
                new Dictionary<string, object>
                {
                    {"__v", "5"},
                    {"scope", groupId},
                    {"readMembers", "true" }
                });

        public static IVstsRequest<Response.Security.IdentityGroup> Groups(string projectId) =>
            new VstsRequest<Response.Security.IdentityGroup>(
                $"/{projectId}/_api/_identity/ReadScopedApplicationGroupsJson",
                new Dictionary<string, object>
                {
                    {"__v", "5"}
                });

        public static IVstsRequest<AddMemberData, object> AddMember(string project) =>
            new VstsRequest<AddMemberData, object>($"/{project}/_api/_identity/AddIdentities",
                new Dictionary<string, object>
                {
                    {"__v", "5"}
                });


        public static IVstsRequest<EditMembersData, object> EditMembership(string project) =>
            new VstsRequest<EditMembersData, object>($"/{project}/_api/_identity/EditMembership",
                new Dictionary<string, object>
                {
                    {"__v", "5"}
                });


        public class EditMembersData
        {
            protected EditMembersData(string groupId)
            {
                GroupId = groupId;
            }
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

        public static IVstsRequest<ManageGroupData, Response.ApplicationGroup> ManageGroup(string project) =>
            new VstsRequest<ManageGroupData, Response.ApplicationGroup>($"/{project}/_api/_identity/ManageGroup",
                new Dictionary<string, object>
                {
                    {"__v", "5"}
                });


        public class ManageGroupData
        {
            public string Name { get; set; }
        }

        public static IVstsRequest<string, object> DeleteIdentity(string project) =>
            new VstsRequest<string, object>($"/{project}/_api/_identity/DeleteIdentity",
                new Dictionary<string, object>
                {
                    {"__v", "5"}
                });

    }
}