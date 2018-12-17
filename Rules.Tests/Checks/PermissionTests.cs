using System;
using System.Collections.Generic;
using RestSharp.Extensions;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Permission = SecurePipelineScan.Rules.Checks.Permission;
using Response = SecurePipelineScan.VstsService.Response;

namespace Rules.Tests.Checks
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
            var permissions = new []
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

                    PermissionId = 2,
                    PermissionDisplayString = "Deny"
                }
            };
            
            Permission.HasNoPermissionToDeleteRepository(permissions).ShouldBeTrue();
        }
    }
}