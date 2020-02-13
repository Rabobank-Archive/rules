using System;
using System.Collections.Generic;
using SecurePipelineScan.VstsService;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using Requests = SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Permissions;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteTheRepositoryTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string RepositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";


        public NobodyCanDeleteTheRepositoryTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public async Task EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync(projectId, RepositoryId)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenAnApplicationGroupHasPermissionToDeleteRepoWithAllow_WhenEvaluating_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();

            InitializeLookupData(client, PermissionId.Allow);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync(_config.Project, RepositoryId)).ShouldBeFalse();
        }

        [Fact]
        public async Task GivenAnApplicationGroupHasPermissionToDeleteRepoWithAllowInHerited_WhenEvaluating_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();

            InitializeLookupData(client, PermissionId.AllowInherited);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync(_config.Project, RepositoryId)).ShouldBeFalse();
        }

        [Fact]
        public async Task GivenNoApplicationGroupHasPermissionToDeleteRepo_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();

            InitializeLookupData(client, PermissionId.Deny);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync(_config.Project, RepositoryId)).ShouldBeTrue();
        }

        [Fact]
        public async Task IgnoreGroupsProjectCollectionAdminAndProjectCollectionServiceAccounts()
        {
            var client = Substitute.For<IVstsRestClient>();

            var applicationGroup1 = new ApplicationGroup
            {
                FriendlyDisplayName = "Project Collection Administrators",
                DisplayName = "blblblablaaProject Collection Administrators",
                TeamFoundationId = "11",
            };

            var applicationGroup2 = new ApplicationGroup
            {
                FriendlyDisplayName = "Project Collection Service Accounts",
                DisplayName = "blblblablaaProject Collection Service Accounts",
                TeamFoundationId = "22",
            };

            var applicationGroup3 = new ApplicationGroup
            {
                FriendlyDisplayName = "Dit is een test",
                DisplayName = "blblblablaaDit is een testy",
                TeamFoundationId = "33",
            };

            var applicationGroups = new Response.ApplicationGroups
            { Identities = new[] { applicationGroup1, applicationGroup2, applicationGroup3 } };

            InitializeLookupData(client, PermissionId.Deny);

            client.GetAsync(Arg.Any<IVstsRequest<Response.ApplicationGroups>>()).Returns(applicationGroups);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync(_config.Project, RepositoryId)).ShouldBeTrue();


            await client
                .DidNotReceive()
                .GetAsync(Arg.Is<IVstsRequest<Response.PermissionsSetId>>(x => x.QueryParams.Contains(new KeyValuePair<string, object>("tfid", "11"))));

            await client
                .DidNotReceive()
                .GetAsync(Arg.Is<IVstsRequest<Response.PermissionsSetId>>(x => x.QueryParams.Contains(new KeyValuePair<string, object>("tfid", "22"))));

            await client
                .Received()
                .GetAsync(Arg.Is<IVstsRequest<Response.PermissionsSetId>>(x => x.QueryParams.Contains(new KeyValuePair<string, object>("tfid", "33"))));

        }

        [Fact]
        public async Task ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            await ManagePermissions
                .ForRepository(client, projectId, RepositoryId)
                .Permissions(512)
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync(projectId, RepositoryId))
                .ShouldBe(false);

            await rule.ReconcileAsync(projectId, RepositoryId);
            await Task.Delay(TimeSpan.FromSeconds(10));

            (await rule.EvaluateAsync(projectId, RepositoryId))
                .ShouldBe(true);
        }

        [Fact]
        public async Task GivenPermissionIsAllowWhenFixPermissionIsUpdatedToDeny()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeLookupData(client, PermissionId.Allow);

            var rule = new NobodyCanDeleteTheRepository(client);
            await rule.ReconcileAsync("TAS", "123");

            await client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<Requests.Permissions.UpdateWrapper, object>>(), Arg.Is<Requests.Permissions.UpdateWrapper>(x =>
                    x.UpdatePackage.Contains("123") &&
                    x.UpdatePackage.Contains(@"PermissionId"":2")));
        }

        [Fact]
        public async Task GivenPermissionIsDeny_WhenFixPermission_IsNotUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeLookupData(client, PermissionId.Deny);


            var rule = new NobodyCanDeleteTheRepository(client);
            await rule.ReconcileAsync("TAS", "123");

            await client
                .DidNotReceive()
                .PostAsync(Arg.Any<IVstsRequest<Requests.Permissions.UpdateWrapper, object>>(), Arg.Any<Requests.Permissions.UpdateWrapper>());
        }

        [Fact]
        public async Task GivenPermissionIsInheritedDeny_WhenFixPermission_IsNotUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeLookupData(client, PermissionId.DenyInherited);


            var rule = new NobodyCanDeleteTheRepository(client);
            await rule.ReconcileAsync("TAS", "123");

            await client
                .DidNotReceive()
                .PostAsync(Arg.Any<IVstsRequest<Requests.Permissions.UpdateWrapper, object>>(), Arg.Any<Requests.Permissions.UpdateWrapper>());
        }

        [Fact]
        public async Task GivenPermissionIsNotSet_WhenFixPermission_IsNotUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializeLookupData(client, PermissionId.NotSet);


            var rule = new NobodyCanDeleteTheRepository(client);
            await rule.ReconcileAsync("TAS", "123");

            await client
                .DidNotReceive()
                .PostAsync(Arg.Any<IVstsRequest<Requests.Permissions.UpdateWrapper, object>>(), Arg.Any<Requests.Permissions.UpdateWrapper>());
        }

        private void InitializeLookupData(IVstsRestClient client, int permissionId)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            client.GetAsync(Arg.Any<IVstsRequest<Response.ProjectProperties>>()).Returns(fixture.Create<Response.ProjectProperties>());
            client.GetAsync(Arg.Any<IVstsRequest<Response.ApplicationGroups>>()).Returns(fixture.Create<Response.ApplicationGroups>());

            client.GetAsync(Arg.Any<IVstsRequest<Response.PermissionsSetId>>()).Returns(new Response.PermissionsSetId()
            {
                Permissions = new[] { new Response.Permission { DisplayName = "Delete repository", PermissionBit = 512, PermissionId = permissionId, PermissionToken = "repoV2/53410703-e2e5-4238-9025-233bd7c811b3/123" }, }
            });
        }
    }
}