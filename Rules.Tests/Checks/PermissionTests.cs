using Shouldly;
using System.Collections.Generic;
using Xunit;
using Permission = SecurePipelineScan.Rules.Checks.Permission;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests.Checks
{
    public class PermissionTests
    {
        [Fact]
        public void EmptyPermissionToDeleteRepositoryShouldBeFalse()
        {
            Permission.HasNoPermissionToDeleteRepository(new List<Response.Permission>())
                .ShouldBeFalse();
        }

        [Fact]
        public void EmptyPermissionToManageRepositoryPermissionShouldBeFalse()
        {
            Permission.HasNoPermissionToManageRepositoryPermissions(new List<Response.Permission>())
                .ShouldBeFalse();
        }

        [Fact]
        public void NoPermissionToManageRepositoryPermissionsShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission
                {
                    PermissionBit = 8192,
                    DisplayName = "Manage permissions",

                    PermissionId = 2,
                    PermissionDisplayString = "Deny"
                    }
            };
            Permission.HasNoPermissionToManageRepositoryPermissions(permissions).ShouldBeTrue();
        }

        [Fact]
        public void NoPermissionToDeleteRepositoryShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 512,
                    DisplayName = "Delete Repository",

                    PermissionId = 4,
                    PermissionDisplayString = "Deny (inherited)"
                }
            };

            Permission.HasNoPermissionToDeleteRepository(permissions).ShouldBeTrue();
        }

        [Fact]
        public void NoPermissionToAdministerBuildPermissionsShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 16384,
                    DisplayName = "Administer build permissions",

                    PermissionId = 4,
                    PermissionDisplayString = "Deny (inherited)"
                }
            };

            Permission.HasNoPermissionToAdministerBuildPermissions(permissions).ShouldBeTrue();
        }
        
        [Fact]
        public void NoPermissionToDeleteBuildDefinitionShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 4096,
                    DisplayName = "Delete build definition",

                    PermissionId = 2,
                    PermissionDisplayString = "Deny"
                }
            };

            Permission.HasNoPermissionToDeleteBuildDefinition(permissions).ShouldBeTrue();
        }


        [Fact]
        public void NoPermissionToDestroyBuildsShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 32,
                    DisplayName = "Destroy builds",

                    PermissionId = 4,
                    PermissionDisplayString = "Deny (inherited)"
                }
            };

            Permission.HasNoPermissionToDestroyBuilds(permissions).ShouldBeTrue();
        }

        [Fact]
        public void NoPermissionToDeleteBuildsShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 8,
                    DisplayName = "Delete builds",

                    PermissionId = 4,
                    PermissionDisplayString = "Deny (inherited)"
                }
            };

            Permission.HasNoPermissionToDeleteBuilds(permissions).ShouldBeTrue();
        }
    }
}