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
            Permission.HasNotSetToManageRepositoryPermissions(new List<Response.Permission>())
                .ShouldBeFalse();
        }

        [Fact]
        public void NotSetToManageRepositoryPermissionsShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission
                {
                    PermissionBit = 8192,
                    DisplayName = "Manage permissions",

                    PermissionId = 0,
                    PermissionDisplayString = "Not Set"
                    }
            };
            Permission.HasNotSetToManageRepositoryPermissions(permissions).ShouldBeTrue();
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
        public void EmptyPermissionToAdministerBuildPermissionsShouldBeFalse()
        {
            Permission.HasNoPermissionToAdministerBuildPermissions(new List<Response.Permission>())
                .ShouldBeFalse();
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
        public void NotSetToDeleteBuildDefinitionShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 4096,
                    DisplayName = "Delete build definition",

                    PermissionId = 0,
                    PermissionDisplayString = "Not Set"
                }
            };

            Permission.HasNotSetToDeleteBuildDefinition(permissions).ShouldBeTrue();
        }


        [Fact]
        public void EmptyPermissionToDeleteBuildDefinitionShouldBeFalse()
        {
            Permission.HasNoPermissionToDeleteBuildDefinition(new List<Response.Permission>())
                .ShouldBeFalse();
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
        public void NotSetToDestroyBuildsShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 32,
                    DisplayName = "Destroy builds",

                    PermissionId = 0,
                    PermissionDisplayString = "Not Set"
                }
            };

            Permission.HasNotSetToDestroyBuilds(permissions).ShouldBeTrue();
        }

        
        [Fact]
        public void EmptyPermissionToDestroyBuildsShouldBeFalse()
        {
            Permission.HasNoPermissionToDestroyBuilds(new List<Response.Permission>())
                .ShouldBeFalse();
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

        [Fact]
        public void NotSetToDeleteBuildsShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 8,
                    DisplayName = "Delete builds",

                    PermissionId = 0,
                    PermissionDisplayString = "Not Set"
                }
            };

            Permission.HasNotSetToDeleteBuilds(permissions).ShouldBeTrue();
        }

        
        [Fact]
        public void EmptyPermissionToDeleteBuildsShouldBeFalse()
        {
            Permission.HasNoPermissionToDeleteBuilds(new List<Response.Permission>())
                .ShouldBeFalse();
        }

        [Fact]
        public void NoPermissionsToAdministerReleasePermissionsShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 512,
                    DisplayName = "Administer release permissions",

                    PermissionId = 4,
                    PermissionDisplayString = "Deny (inherited)"
                }
            };
            
            Permission.HasNoPermissionToAdministerReleasePermissions(permissions).ShouldBeTrue();
        }

        [Fact]
        public void NoPermissionsToDeleteReleasePipelineShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 4,
                    DisplayName = "Delete release pipeline",

                    PermissionId = 4,
                    PermissionDisplayString = "Deny (inherited)"
                }
            };
            Permission.HasNoPermissionToDeleteReleasePipeline(permissions).ShouldBeTrue();
        }

        [Fact]
        public void NoPermissionsToDeleteReleaseShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 1024,
                    DisplayName = "Delete releases",

                    PermissionId = 4,
                    PermissionDisplayString = "Deny (inherited)"
                }
            };
            Permission.HasNoPermissionToDeleteReleases(permissions).ShouldBeTrue();
        }

        [Fact]
        public void NoPermissionToManageReleaseApproversShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 8,
                    DisplayName = "Manage release approvers",

                    PermissionId = 2,
                    PermissionDisplayString = "Deny"
                }
            };
            Permission.HasNoPermissionToManageReleaseApprovers(permissions).ShouldBeTrue();   
        }
        
        [Fact]
        public void HasPermissionToManageReleaseApproversShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 8,
                    DisplayName = "Manage release approvers",

                    PermissionId = 1,
                    PermissionDisplayString = "Allow"
                }
            };
            Permission.HasPermissionToManageReleaseApprovers(permissions).ShouldBeTrue();   
        }
        
        [Fact]
        public void NoPermissionToCreateReleasesShouldBeTrue()
        {
            var permissions = new[]
            {
                new Response.Permission()
                {
                    PermissionBit = 64,
                    DisplayName = "Create releases",

                    PermissionId = 4,
                    PermissionDisplayString = "Deny (inherited)"
                }
            };
            Permission.HasNoPermissionToCreateReleases(permissions).ShouldBeTrue();   
        }
    
        [Fact]
        public void HasNoPermissionToDeleteTeamProjectShouldBeTrue()
        {
            var permissions = new Response.PermissionsProjectId
            {
                Security = new Response.PermissionsSetId()
                {
                    Permissions = new[] {
                            new Response.Permission {
                            PermissionBit = 4,
                            DisplayName = "Delete team project",

                            PermissionId = 2,
                            PermissionDisplayString = "Deny"
                            }
                        }
                }
            };


            Permission.HasNoPermissionToDeleteTeamProject(permissions.Security.Permissions).ShouldBeTrue();
        }

        [Fact]
        public void HasNoPermissionToPermanentlyDeleteWorkitemsShouldBeTrue()
        {
            var permissions = new Response.PermissionsProjectId
            {
                Security = new Response.PermissionsSetId()
                {
                    Permissions = new[] {
                            new Response.Permission {
                            PermissionBit = 32768,
                            DisplayName = "Permanently delete work items",

                            PermissionId = 2,
                            PermissionDisplayString = "Deny"
                            }
                        }
                }
            };


            Permission.HasNoPermissionToPermanentlyDeleteWorkitems(permissions.Security.Permissions).ShouldBeTrue();
        }

        [Fact]
        public void HasNoPermissionToManageProjectPropertiesShouldBeTrue()
        {
            var permissions = new Response.PermissionsProjectId
            {
                Security = new Response.PermissionsSetId()
                {
                    Permissions = new[] {
                            new Response.Permission {
                            PermissionBit = 131072,
                            DisplayName = "Manage project properties",

                            PermissionId = 2,
                            PermissionDisplayString = "Deny"
                            }
                        }
                }
            };

            Permission.HasNoPermissionToManageProjectProperties(permissions.Security.Permissions).ShouldBeTrue();
        }
    }
}