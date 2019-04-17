using SecurePipelineScan.VstsService;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using RestSharp;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteTheRepositoryTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private string repoSoxCompliantDemo = "3167b64e-c72b-4c55-84eb-986ac62d0dec";


        public NobodyCanDeleteTheRepositoryTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public void EvaluateIntegrationTest()
        {
            var rule = new NobodyCanDeleteTheRepository(new VstsRestClient(_config.Organization, _config.Token));
            rule.Evaluate(_config.Project, repoSoxCompliantDemo).ShouldBeTrue();
        }

        [Fact]
        public void GivenAnApplicationGroupHasPermissionToDeleteRepo_WhenEvaluating_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            
            InitializeLookupData(client, PermissionId.Allow);
            
            var rule = new NobodyCanDeleteTheRepository(client);
            rule.Evaluate(_config.Project, repoSoxCompliantDemo).ShouldBeFalse();
        }
        
        [Fact]
        public void GivenNoApplicationGroupHasPermissionToDeleteRepo_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();

            InitializeLookupData(client, PermissionId.Deny);

            var rule = new NobodyCanDeleteTheRepository(client);
            rule.Evaluate(_config.Project, repoSoxCompliantDemo).ShouldBeTrue();
        }

        private void InitializeLookupData(IVstsRestClient client, int permissionId)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            client.Get(Arg.Any<IVstsRestRequest<ProjectProperties>>()).Returns(fixture.Create<ProjectProperties>());
            client.Get(Arg.Any<IVstsRestRequest<ApplicationGroups>>()).Returns(fixture.Create<ApplicationGroups>());

            client.Get(Arg.Any<IVstsRestRequest<Multiple<SecurityNamespace>>>())
                .Returns(new Multiple<SecurityNamespace>()
                {
                    Count = 1, Value = new SecurityNamespace[1]
                    {
                        new SecurityNamespace()
                            {DisplayName = "Git Repositories"}
                    }
                });

            client.Get(Arg.Any<IVstsRestRequest<PermissionsSetId>>()).Returns(new PermissionsSetId()
            {
                Permissions = new[] {new Permission() {DisplayName = "Delete repository", PermissionId = permissionId},}
            });
        }
    }
}