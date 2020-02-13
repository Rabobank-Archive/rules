using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using Shouldly;
using Xunit;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Permissions;

namespace SecurePipelineScan.VstsService.Tests
{
    public static class UpdatePermissionsTests
    {
        [Fact]
        public static async Task Update()
        {
            // Arrange 
            var group = new Response.ApplicationGroup();
            var permission = new Response.Permission();
            var permissions = new Response.PermissionsSetId { Permissions =  new[] { permission,}};
            
            var mock = Substitute.For<IFor>();
            mock
                .IdentitiesAsync()
                .Returns(new Response.ApplicationGroups { Identities = new[] { group } });
            mock
                .PermissionSetAsync(group)
                .Returns(permissions);
            
            // Act
            var target = new ManagePermissions(mock);
            await target.SetToAsync(4);

            // Assert
            await mock
                .Received()
                .UpdateAsync(group, permissions, permission);
            
            permission
                .PermissionId
                .ShouldBe(4);
        }
        
        [Fact]
        public static async Task Ignore()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.ApplicationGroup>(ctx => ctx
                .With(x => x.FriendlyDisplayName, "some-name"));
            
            var mock = fixture.Create<IFor>();
            
            // Act
            var target = new ManagePermissions(mock);
            await target.Ignore("some-name").SetToAsync(4);

            // Assert
            await mock
                .Received(0)
                .UpdateAsync(Arg.Any<Response.ApplicationGroup>(), Arg.Any<Response.PermissionsSetId>(), Arg.Any<Response.Permission>());
        }
        
        [Fact]
        public static async Task For()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.ApplicationGroup>(ctx => ctx
                .With(x => x.FriendlyDisplayName, "some-name"));
            
            var mock = fixture.Create<IFor>();
            
            // Act
            await new ManagePermissions(mock).For("some-name").SetToAsync(4);

            // Assert
            await mock
                .Received()
                .UpdateAsync(Arg.Any<Response.ApplicationGroup>(), Arg.Any<Response.PermissionsSetId>(), Arg.Any<Response.Permission>());
        }
        
        [Fact]
        public static async Task ForIgnoreOthers()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.ApplicationGroup>(ctx => ctx
                .With(x => x.FriendlyDisplayName, "some-name"));
            
            var mock = fixture.Create<IFor>();
            
            // Act
            var target = new ManagePermissions(mock);
            await target.For("some-other").SetToAsync(4);

            // Assert
            await mock
                .Received(0)
                .UpdateAsync(Arg.Any<Response.ApplicationGroup>(), Arg.Any<Response.PermissionsSetId>(), Arg.Any<Response.Permission>());
        }
        
        [Fact]
        public static async Task Permissions()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.Permission>(ctx => ctx
                .With(x => x.PermissionBit, 1234));
            
            var mock = fixture.Create<IFor>();
            
            // Act
            var target = new ManagePermissions(mock);
            await target.Permissions(1234).SetToAsync(4);

            // Assert
            await mock
                .Received()
                .UpdateAsync(Arg.Any<Response.ApplicationGroup>(), Arg.Any<Response.PermissionsSetId>(), Arg.Any<Response.Permission>());
        }
        
        [Fact]
        public static async Task PermissionsIgnoreOther()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.Permission>(ctx => ctx
                .With(x => x.PermissionBit, 1));
            
            var mock = fixture.Create<IFor>();
            
            // Act
            var target = new ManagePermissions(mock);
            await target.Permissions(1234).SetToAsync(4);

            // Assert
            await mock
                .Received(0)
                .UpdateAsync(Arg.Any<Response.ApplicationGroup>(), Arg.Any<Response.PermissionsSetId>(), Arg.Any<Response.Permission>());
        }
        
        [Fact]
        public static async Task Allow()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.Permission>(ctx => ctx
                .With(x => x.PermissionId, 1));
            
            var mock = fixture.Create<IFor>();
            
            // Act
            var target = new ManagePermissions(mock);
            await target.Allow(1).SetToAsync(4);

            // Assert
            await mock
                .Received(0)
                .UpdateAsync(Arg.Any<Response.ApplicationGroup>(), Arg.Any<Response.PermissionsSetId>(), Arg.Any<Response.Permission>());
        }
        
        [Fact]
        public static async Task Validate()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.Permission>(ctx => ctx
                .With(x => x.PermissionId, 4)
                .With(x => x.PermissionBit, 1234));
            
            var mock = fixture.Create<IFor>();        // Act
            var target = new ManagePermissions(mock);
            var result = await target
                .Permissions(1324)
                .Allow(4)
                .ValidateAsync();

            // Assert
            result.ShouldBeTrue();
        }
        
        [Fact]
        public static async Task ValidateFalse()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.Permission>(ctx => ctx
                .With(x => x.PermissionId, 1)
                .With(x => x.PermissionBit, 1234));
            
            var mock = fixture.Create<IFor>();        // Act
            var target = new ManagePermissions(mock);
            var result = await target
                .Permissions(1234)
                .Allow(4)
                .ValidateAsync();

            // Assert
            result.ShouldBeFalse();
        }
        
        [Fact]
        public static async Task ValidateIgnore()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.ApplicationGroup>(ctx => ctx
                .With(x => x.FriendlyDisplayName, "some-name"));
            
            fixture.Customize<Response.Permission>(ctx => ctx
                .With(x => x.PermissionId, 1)
                .With(x => x.PermissionBit, 1234));
            
            var mock = fixture.Create<IFor>();        // Act
            var target = new ManagePermissions(mock);
            var result = await target
                .Ignore("some-name")
                .Permissions(1234)
                .Allow(4)
                .ValidateAsync();

            // Assert
            result.ShouldBeTrue();
        }
        
        [Fact]
        public static async Task ValidateOtherPermission()
        {
            // Arrange 
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
            fixture.Customize<Response.ApplicationGroup>(ctx => ctx
                .With(x => x.FriendlyDisplayName, "some-name"));
            
            fixture.Customize<Response.Permission>(ctx => ctx
                .With(x => x.PermissionId, 1)
                .With(x => x.PermissionBit, 1234));
            
            var mock = fixture.Create<IFor>();        // Act
            var target = new ManagePermissions(mock);
            var result = await target
                .Permissions(4444)
                .Allow(4)
                .ValidateAsync();

            // Assert
            result.ShouldBeTrue();
        }
    }
}