using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using SecurityNamespace = SecurePipelineScan.VstsService.Response.SecurityNamespace;

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
        public void GivenAnApplicationGroupHasPermissionToDeleteRepoWithAllow_WhenEvaluating_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            
            InitializeLookupData(client, PermissionId.Allow);
            
            var rule = new NobodyCanDeleteTheRepository(client);
            rule.Evaluate(_config.Project, repoSoxCompliantDemo).ShouldBeFalse();
        }
        
        [Fact]
        public void GivenAnApplicationGroupHasPermissionToDeleteRepoWithAllowInHerited_WhenEvaluating_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            
            InitializeLookupData(client, PermissionId.AllowInherited);
            
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

        [Fact]
        public void IgnoreGroupsProjectCollectionAdminAndProjectCollectionServiceAccounts()
        {
            var client = Substitute.For<IVstsRestClient>();
            
            var applicationGroup1 = new ApplicationGroup
            {
                FriendlyDisplayName = "Project Collection Administrators",
                DisplayName = "blblblablaaProject Collection Administrators",
                TeamFoundationId= "11",
            };

            var applicationGroup2 = new ApplicationGroup
            {
                FriendlyDisplayName = "Project Collection Service Accounts",
                DisplayName = "blblblablaaProject Collection Service Accounts",
                TeamFoundationId= "22",
            };
            
            var applicationGroup3 = new ApplicationGroup
            {
                FriendlyDisplayName = "Dit is een test",
                DisplayName = "blblblablaaDit is een testy",
                TeamFoundationId= "33",
            };

            var applicationGroups = new ApplicationGroups
                {Identities = new[] {applicationGroup1, applicationGroup2, applicationGroup3}};
            
            InitializeLookupData(client, PermissionId.Deny);
            
            client.Get(Arg.Any<IVstsRestRequest<ApplicationGroups>>()).Returns(applicationGroups);
            
            var rule = new NobodyCanDeleteTheRepository(client);
            rule.Evaluate(_config.Project, repoSoxCompliantDemo).ShouldBeTrue();
            
            
            client
                .DidNotReceive()
                .Get(Arg.Is<IVstsRestRequest<PermissionsSetId>>(x => x.Uri.Contains("tfid=11")));
            
            client
                .DidNotReceive()
                .Get(Arg.Is<IVstsRestRequest<PermissionsSetId>>(x => x.Uri.Contains("tfid=22")));
            
            client
                .Received()
                .Get(Arg.Is<IVstsRestRequest<PermissionsSetId>>(x => x.Uri.Contains("tfid=33")));

        }

        [Fact]
        public void ReconcileIntegrationTest()
        {
            var rule = new NobodyCanDeleteTheRepository(new VstsRestClient(_config.Organization, _config.Token));
            rule.Reconcile(_config.Project, repoSoxCompliantDemo);
        }

        [Fact]
        public void GivenPermissionIsAllowWhenFixPermissionIsUpdatedToDeny()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeLookupData(client, PermissionId.Allow);
            
            var data = new List<object>();
            client
                .Post(Arg.Do<IVstsPostRequest<object>>(x => data.Add(x.Body)));

            var rule = new NobodyCanDeleteTheRepository(client);
            rule.Reconcile("TAS", "123");
            
            data
                .OfType<VstsService.Requests.Permissions.UpdateWrapper>() // Couldn't help it but the API is ugly here and requires some wrapping object with a JSON string as content
                .ShouldContain(x => 
                    x.UpdatePackage.Contains("123") &&
                    x.UpdatePackage.Contains(@"PermissionId"":2"));
        }
        
        [Fact]
        public void GivenPermissionIsDeny_WhenFixPermission_IsNotUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeLookupData(client, 2);
            

            var rule = new NobodyCanDeleteTheRepository(client);
            rule.Reconcile("TAS", "123");

            client
                .DidNotReceive()
                .Post(Arg.Any<IVstsPostRequest<object>>());
        }
        
        [Fact]
        public void GivenPermissionIsInheritedDeny_WhenFixPermission_IsNotUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeLookupData(client, 4);
            

            var rule = new NobodyCanDeleteTheRepository(client);
            rule.Reconcile("TAS", "123");

            client
                .DidNotReceive()
                .Post(Arg.Any<IVstsPostRequest<object>>());
        }
        
        [Fact]
        public void GivenPermissionIsNotSet_WhenFixPermission_IsNotUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeLookupData(client, 0);
            

            var rule = new NobodyCanDeleteTheRepository(client);
            rule.Reconcile("TAS", "123");

            client
                .DidNotReceive()
                .Post(Arg.Any<IVstsPostRequest<object>>());
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
                        new SecurityNamespace { DisplayName = "Git Repositories" }
                    }
                });

            client.Get(Arg.Any<IVstsRestRequest<PermissionsSetId>>()).Returns(new PermissionsSetId()
            {
                Permissions = new[] {new Permission() {DisplayName = "Delete repository", PermissionId = permissionId, PermissionToken = "repoV2/53410703-e2e5-4238-9025-233bd7c811b3/123"},}
            });
        }
    }
}