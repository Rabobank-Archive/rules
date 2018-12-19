using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NSubstitute;
using RestSharp;
using SecurePipelineScan.VstsService;
using Xunit;
using Shouldly;

using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.GroupMemberShipCheck.Tests
{
    public class GroupMembershipTests
    {
        [Fact]
        public void OneGroupmemberIsOk()
        {
            string groupName = "Project Administrators";
            List<string> okProjectNames = new List<string>();
            List<string> notOkProjectNames = new List<string>();
            NameValueCollection notOkProjectMembers = new NameValueCollection();

            var client = SetupClient(
                new Response.Security.Identity { FriendlyDisplayName = "Project Administrators", DisplayName = "Project Administrators" },
                new Response.Security.Identity { FriendlyDisplayName = "Member 1", DisplayName = "Member 1" }
            );

            var checker = new GroupMemberShipChecker();
            CheckResults result = checker.Execute(client, groupName, okProjectNames, notOkProjectNames, notOkProjectMembers);
            result.ShouldNotBeNull();
        }

        [Fact]
        public void MultipleIdentitiesAreNotOkay()
        {
            var client = SetupClient(
                new Response.Security.Identity { FriendlyDisplayName = "Project Administrators", DisplayName = "Project Administrators" },
                new Response.Security.Identity { FriendlyDisplayName = "Member 1", DisplayName = "Member 1" },
                new Response.Security.Identity { FriendlyDisplayName = "Member 2", DisplayName = "Member 2" }
            );

            string groupName = "Project Administrators";
            List<string> okProjectNames = new List<string>();
            List<string> notOkProjectNames = new List<string>();
            NameValueCollection notOkProjectMembers = new NameValueCollection();
            var checker = new GroupMemberShipChecker();
            CheckResults result = checker.Execute(client, groupName, okProjectNames, notOkProjectNames, notOkProjectMembers);
            result.ShouldNotBeNull();
        }

        private static IVstsRestClient SetupClient(params Response.Security.Identity[] identities)
        {
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.Project>>>())
                .Returns( new Response.Multiple<Response.Project>
                    {
                        Value = new[] { new Response.Project { Id = "asdf" } }
                    }
                );

            client
                .Get(Arg.Any<IVstsRestRequest<Response.Security.IdentityGroup>>())
                .Returns(
                    new Response.Security.IdentityGroup
                    {
                        Identities = identities
                    }
               );

            return client;
        }
    }
}
