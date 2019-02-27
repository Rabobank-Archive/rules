using NSubstitute;
using SecurePipelineScan.VstsService;
using Shouldly;
using System.Linq;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    public class SecurityDataFetcherTests
    {
        [Fact]
        public void FetchSecurityPermissions_ShouldHaveGlobals()
        {
            IVstsRestClient client = CreateMockedClient();

            var sut = new SecurityDataFetcher(client);

            var result = sut.FetchSecurityPermissions("projectName");

            result.ProjectName.ShouldBe("projectName");
            result.GlobalPermissions.ShouldNotBeNull();

            result.GlobalPermissions.Keys.ShouldContain("tfid_actualGroup1");
            result.GlobalPermissions.Keys.ShouldContain("tfid_actualGroup2");

            var actualGroup1 = result.GlobalPermissions.Single(x => x.Key == "tfid_actualGroup1");
            var actualGroup2 = result.GlobalPermissions.Single(x => x.Key == "tfid_actualGroup2");

            actualGroup1.Value.ShouldContain(x => x.PermissionBit == 32 && x.PermissionId == Common.PermissionId.Allow);
            actualGroup1.Value.ShouldContain(x => x.PermissionBit == 256 && x.PermissionId == Common.PermissionId.Deny);

            actualGroup2.Value.ShouldContain(x => x.PermissionBit == 64 && x.PermissionId == Common.PermissionId.Allow);
            actualGroup2.Value.ShouldContain(x => x.PermissionBit == 512 && x.PermissionId == Common.PermissionId.Deny);
        }

        private static IVstsRestClient CreateMockedClient()
        {
            IVstsRestClient client = Substitute.For<IVstsRestClient>();

            MockApplicationGroups(client);
            MockSecurityNamespaces(client);
            MockPermissions(client);

            return client;
        }

        private static void MockSecurityNamespaces(IVstsRestClient client)
        {
            var securityNamespace1 = new Response.SecurityNamespace
            {
                DisplayName = "Git Repositories",
                NamespaceId = "1234"
            };

            var securityNamespace2 = new Response.SecurityNamespace
            {
                Name = "Build",
                NamespaceId = "5678"
            };

            var securityNamespace3 = new Response.SecurityNamespace
            {
                Name = "ReleaseManagement",
                NamespaceId = "8912",
                Actions = new[] {
                    new Response.NamespaceAction { Name = "ViewReleaseDefinition" }
                }
            };
            var securityNamespaces =
                new Response.Multiple<Response.SecurityNamespace>(
                    securityNamespace1,
                    securityNamespace2,
                    securityNamespace3);

            client.Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.SecurityNamespace>>>())
                .Returns(securityNamespaces);
        }

        private static void MockApplicationGroups(IVstsRestClient client)
        {
            var applicationGroup1 = new Response.ApplicationGroup
            { DisplayName = "actualGroup1", TeamFoundationId = "tfid_actualGroup1", };
            var applicationGroup2 = new Response.ApplicationGroup
            { DisplayName = "actualGroup2", TeamFoundationId = "tfid_actualGroup2" };

            var applicationGroups = new Response.ApplicationGroups
            {
                Identities = new[]
                {
                    applicationGroup1, applicationGroup2
                }
            };

            client.Get(Arg.Any<IVstsRestRequest<Response.ApplicationGroups>>()).
                Returns(applicationGroups);
        }

        private static void MockRepositoryPermissions(IVstsRestClient client)
        {
            Response.PermissionsSetId permissionSetIds = new Response.PermissionsSetId
            {
                CurrentTeamFoundationId = "tfid_actualGroup1",
                Permissions = new[]
    {
                    new Response.Permission()
                    {
                        DisplayName = "Contribute Code",
                        PermissionBit = 64,
                        PermissionDisplayString = "ContributeCode DisplayString",
                        PermissionId = 2,
                    },
                    new Response.Permission()
                    {
                        DisplayName = "View Code",
                        PermissionBit = 128,
                        PermissionDisplayString = "View Code DisplayString",
                        PermissionId = 1,
                    },
                }
            };

            client.Get(Arg.Any<IVstsRestRequest<Response.PermissionsSetId>>())
                .Returns(permissionSetIds);
        }

        private static void MockPermissions(IVstsRestClient client)
        {
            Response.PermissionsProjectId permissionsProjectIdGroup1 = new Response.PermissionsProjectId
            {
                Security = new Response.PermissionsSetId
                {
                    CurrentTeamFoundationId = "tfid_actualGroup1",
                    Permissions = new[]
                    {
                        new Response.Permission
                        {
                            DisplayName = "View",
                            PermissionBit = 32,
                            PermissionDisplayString = "View DisplayString",
                            PermissionId = 1,
                        },
                        new Response.Permission
                        {
                            DisplayName = "Edit",
                            PermissionBit = 256,
                            PermissionDisplayString = "Edit DisplayString",
                            PermissionId = 2,
                        }
                    }
                }
            };

            Response.PermissionsProjectId permissionsProjectIdGroup2 = new Response.PermissionsProjectId
            {
                Security = new Response.PermissionsSetId
                {
                    CurrentTeamFoundationId = "tfid_actualGroup2",
                    Permissions = new[]
                    {
                        new Response.Permission
                        {
                            DisplayName = "View",
                            PermissionBit = 64,
                            PermissionDisplayString = "View DisplayString",
                            PermissionId = 1,
                        },
                        new Response.Permission
                        {
                            DisplayName = "Edit",
                            PermissionBit = 512,
                            PermissionDisplayString = "Edit DisplayString",
                            PermissionId = 2,
                        }
                    }
                }
            };

            client.Get(VstsService.Requests.Permissions.PermissionsGroupProjectId(Arg.Any<string>(), Arg.Any<string>())
                as IVstsRestRequest<Response.PermissionsProjectId>).
                Returns(permissionsProjectIdGroup1);

            client.Get(VstsService.Requests.Permissions.PermissionsGroupProjectId(Arg.Any<string>(), "tfid_actualGroup2")
                as IVstsRestRequest<Response.PermissionsProjectId>).
                Returns(permissionsProjectIdGroup2);

            client.Get(Arg.Any<IVstsRestRequest<Response.PermissionsProjectId>>()).
                Returns(permissionsProjectIdGroup1, permissionsProjectIdGroup2);
        }
    }
}