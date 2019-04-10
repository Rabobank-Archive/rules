
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using NSubstitute;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    public class ProjectAdministratorsOnlyContainRaboAdministratorsTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public ProjectAdministratorsOnlyContainRaboAdministratorsTests(TestConfig config)
        {
            _config = config;
        }
        
        [Fact]
        public void IntegrationTest()
        {           
           var rule = new ProjectAdministratorsOnlyContainRaboAdministrators(new VstsRestClient(_config.Organization, _config.Token));
           rule.Evaluate(_config.Project).ShouldBeTrue();
        }
        
        [Fact]
        public void EmptyMembersListForProjectAdministratorsStillTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeProjectAdministratorsLookup(client);            
            InitializeMembersLookup(client, Enumerable.Empty<Response.ApplicationGroup>());
            
            var rule = new ProjectAdministratorsOnlyContainRaboAdministrators(client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }
        
        [Fact]
        public void OtherMemberEvaluatesFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeProjectAdministratorsLookup(client);
            InitializeMembersLookup(client, new[]
            {
                new Response.ApplicationGroup()
            });
            
            var rule = new ProjectAdministratorsOnlyContainRaboAdministrators(client);
            rule.Evaluate(_config.Project).ShouldBeFalse();
        }

        private static void InitializeMembersLookup(IVstsRestClient client, IEnumerable<Response.ApplicationGroup> members)
        {
            client
                .Get(Arg.Is<IVstsRestRequest<Response.ApplicationGroups>>(x =>
                    x.Uri.Contains("ReadGroupMembers")))
                .Returns(new Response.ApplicationGroups
                {
                    Identities = members
                });
        }

        private static void InitializeProjectAdministratorsLookup(IVstsRestClient client)
        {
            client
                .Get(Arg.Is<IVstsRestRequest<Response.ApplicationGroups>>(x =>
                    x.Uri.Contains("ReadScopedApplicationGroupsJson")))
                .Returns(new Response.ApplicationGroups
                {
                    Identities = new[] {new Response.ApplicationGroup {FriendlyDisplayName = "Project Administrators"}}
                });
        }
    }
}