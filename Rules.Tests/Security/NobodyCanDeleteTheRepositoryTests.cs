using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteTheRepositoryTests
    {
        private const string RepositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";

        
        [Fact]
        public async Task GivenAnApplicationGroupHasPermissionToDeleteRepoWithAllow_WhenEvaluating_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();

            InitializeLookupData(client, PermissionId.Allow);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync("", RepositoryId)).ShouldBeFalse();
        }

        [Fact]
        public async Task GivenAnApplicationGroupHasPermissionToDeleteRepoWithAllowInHerited_WhenEvaluating_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();

            InitializeLookupData(client, PermissionId.AllowInherited);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync("", RepositoryId)).ShouldBeFalse();
        }

        [Fact]
        public async Task GivenNoApplicationGroupHasPermissionToDeleteRepo_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();

            InitializeLookupData(client, PermissionId.Deny);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync("", RepositoryId)).ShouldBeTrue();
        }

        [Fact]
        public async Task IgnoreGroupsProjectCollectionAdminAndProjectCollectionServiceAccounts()
        {
            var client = Substitute.For<IVstsRestClient>();

            var applicationGroup1 = new ApplicationGroup
            {
                FriendlyDisplayName = "Project Collection Administrators",
                DisplayName = "blblblablaaProject Collection Administrators",
                TeamFoundationId = "11"
            };

            var applicationGroup2 = new ApplicationGroup
            {
                FriendlyDisplayName = "Project Collection Service Accounts",
                DisplayName = "blblblablaaProject Collection Service Accounts",
                TeamFoundationId = "22"
            };

            var applicationGroup3 = new ApplicationGroup
            {
                FriendlyDisplayName = "Dit is een test",
                DisplayName = "blblblablaaDit is een testy",
                TeamFoundationId = "33"
            };

            var applicationGroups = new ApplicationGroups
                {Identities = new[] {applicationGroup1, applicationGroup2, applicationGroup3}};

            InitializeLookupData(client, PermissionId.Deny);

            client.GetAsync(Arg.Any<IVstsRequest<ApplicationGroups>>()).Returns(applicationGroups);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync("", RepositoryId)).ShouldBeTrue();


            await client
                .DidNotReceive()
                .GetAsync(Arg.Is<IVstsRequest<PermissionsSetId>>(x =>
                    x.QueryParams.Contains(new KeyValuePair<string, object>("tfid", "11"))));

            await client
                .DidNotReceive()
                .GetAsync(Arg.Is<IVstsRequest<PermissionsSetId>>(x =>
                    x.QueryParams.Contains(new KeyValuePair<string, object>("tfid", "22"))));

            await client
                .Received()
                .GetAsync(Arg.Is<IVstsRequest<PermissionsSetId>>(x =>
                    x.QueryParams.Contains(new KeyValuePair<string, object>("tfid", "33"))));
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
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),
                    Arg.Is<Permissions.UpdateWrapper>(x =>
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
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),
                    Arg.Any<Permissions.UpdateWrapper>());
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
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),
                    Arg.Any<Permissions.UpdateWrapper>());
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
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),
                    Arg.Any<Permissions.UpdateWrapper>());
        }

        private void InitializeLookupData(IVstsRestClient client, int permissionId)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            client.GetAsync(Arg.Any<IVstsRequest<ProjectProperties>>()).Returns(fixture.Create<ProjectProperties>());
            client.GetAsync(Arg.Any<IVstsRequest<ApplicationGroups>>()).Returns(fixture.Create<ApplicationGroups>());

            client.GetAsync(Arg.Any<IVstsRequest<PermissionsSetId>>()).Returns(new PermissionsSetId
            {
                Permissions = new[]
                {
                    new Permission
                    {
                        DisplayName = "Delete repository", PermissionBit = 512, PermissionId = permissionId,
                        PermissionToken = "repoV2/53410703-e2e5-4238-9025-233bd7c811b3/123"
                    }
                }
            });
        }
    }
}