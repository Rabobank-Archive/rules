using Common;
using NSubstitute;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static Common.PermissionId;
using Permission = Common.Permission;

namespace SecurePipelineScan.Rules.Tests.Reports
{
    public class SecurityReportProcessorTests : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper _output;

        public SecurityReportProcessorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldOutputProjectName()
        {
            ISecurityData data = Substitute.For<ISecurityData>();
            ICompliantValues values = Substitute.For<ICompliantValues>();

            data.ProjectName.Returns("expectedProjectName");

            var sut = new SecurityReportProcessor();
            SecurityReport result = sut.Evaluate(data, values);

            Assert.NotNull(result);

            result.ProjectName.ShouldBe("expectedProjectName");
        }

        [Fact]
        public void ShouldOutputGlobalPermissions()
        {
            ISecurityData data = Substitute.For<ISecurityData>();
            ICompliantValues values = Substitute.For<ICompliantValues>();

            data.ProjectName.Returns("expectedProjectName");

            var fakeData = CreateFakeISecurityData();
            data.GlobalPermissions.Returns(fakeData);

            var sut = new SecurityReportProcessor();
            SecurityReport result = sut.Evaluate(data, values);

            Assert.NotNull(result);
            Assert.NotNull(result.GlobalPermissions);

            result.GlobalPermissions.ShouldContain(x => x.ApplicationGroupName == "group1");
            result.GlobalPermissions.ShouldContain(x => x.ApplicationGroupName == "group2");

            var groupOnePermissions = result.GlobalPermissions.Single(x => x.ApplicationGroupName == "group1");
            var groupTwoPermissions = result.GlobalPermissions.Single(x => x.ApplicationGroupName == "group2");

            groupOnePermissions.Permissions.ShouldContain(x => x.PermissionBit == 1 && x.ActualPermissionId == Allow);
            groupOnePermissions.Permissions.ShouldContain(x => x.PermissionBit == 2 && x.ActualPermissionId == Deny);

            groupTwoPermissions.Permissions.ShouldContain(x => x.PermissionBit == 1 && x.ActualPermissionId == NotSet);
        }

        [Fact]
        public void ShouldMapCompliantValues()
        {
            ISecurityData data = Substitute.For<ISecurityData>();
            ICompliantValues values = Substitute.For<ICompliantValues>();

            data.ProjectName.Returns("expectedProjectName");

            data.GlobalPermissions.Returns(new Dictionary<string, IEnumerable<Permission>>());
            values.GlobalPermissions.Returns(CreateFakeICompliantData());

            var sut = new SecurityReportProcessor();
            SecurityReport result = sut.Evaluate(data, values);

            Assert.NotNull(result);
            Assert.NotNull(result.GlobalPermissions);

            result.GlobalPermissions.ShouldContain(x => x.ApplicationGroupName == "compliantGroup1");

            var compliantGroupOne = result.GlobalPermissions.Single(x => x.ApplicationGroupName == "compliantGroup1");

            compliantGroupOne.Permissions.ShouldContain(x => x.PermissionBit == 1 && x.ShouldBePermissionId == Allow);
            compliantGroupOne.Permissions.ShouldContain(x => x.PermissionBit == 2 && x.ShouldBePermissionId == Deny);
        }

        [Fact]
        public void ShouldMapAndUpdateCompliantValues()
        {
            ISecurityData data = Substitute.For<ISecurityData>();
            ICompliantValues values = Substitute.For<ICompliantValues>();

            data.ProjectName.Returns("expectedProjectName");

            data.GlobalPermissions.Returns(CreateFakeISecurityData());

            IDictionary<string, IEnumerable<Permission>> fakeICompliantValues =
                new Dictionary<string, IEnumerable<Permission>>
                {
                    {
                        "group1",
                        new []{ new Permission(1, Deny) {DisplayName = "Permission 1"}}
                    }
                };

            values.GlobalPermissions.Returns(fakeICompliantValues);

            var sut = new SecurityReportProcessor();
            SecurityReport result = sut.Evaluate(data, values);

            Assert.NotNull(result);
            Assert.NotNull(result.GlobalPermissions);

            result.GlobalPermissions.ShouldContain(x => x.ApplicationGroupName == "group1");

            var compliantGroupOne = result.GlobalPermissions.Single(x => x.ApplicationGroupName == "group1");

            compliantGroupOne.Permissions.ShouldContain(x => x.PermissionBit == 1 && x.ActualPermissionId == Allow && x.ShouldBePermissionId == Deny);
            compliantGroupOne.Permissions.ShouldContain(x => x.PermissionBit == 2 && !x.ShouldBePermissionId.HasValue);
        }

        [Fact]
        public void ShouldMapAndInsertCompliantValues()
        {
            ISecurityData data = Substitute.For<ISecurityData>();
            ICompliantValues values = Substitute.For<ICompliantValues>();

            data.ProjectName.Returns("expectedProjectName");

            data.GlobalPermissions.Returns(CreateFakeISecurityData());

            IDictionary<string, IEnumerable<Permission>> fakeICompliantValues =
                new Dictionary<string, IEnumerable<Permission>>
                {
                    {
                        "group1",
                        new []{ new Permission(64, Deny) { DisplayName = "Permission 64"} }
                    }
                };

            values.GlobalPermissions.Returns(fakeICompliantValues);

            var sut = new SecurityReportProcessor();
            SecurityReport result = sut.Evaluate(data, values);

            Assert.NotNull(result);
            Assert.NotNull(result.GlobalPermissions);

            result.GlobalPermissions.ShouldContain(x => x.ApplicationGroupName == "group1");

            var compliantGroupOne = result.GlobalPermissions.Single(x => x.ApplicationGroupName == "group1");

            compliantGroupOne.Permissions.ShouldContain(x => x.PermissionBit == 64 && !x.ActualPermissionId.HasValue && x.ShouldBePermissionId == Deny);
            compliantGroupOne.Permissions.ShouldContain(x => x.PermissionBit == 2 && !x.ShouldBePermissionId.HasValue);
        }

        [Fact]
        public void ShouldMapApplicationGroupWithDifferentCase()
        {
            ISecurityData data = Substitute.For<ISecurityData>();
            ICompliantValues values = Substitute.For<ICompliantValues>();

            data.ProjectName.Returns("expectedProjectName");

            data.GlobalPermissions.Returns(CreateFakeISecurityData());

            IDictionary<string, IEnumerable<Permission>> fakeICompliantValues =
                new Dictionary<string, IEnumerable<Permission>>
                {
                    {
                        "GROUP1",
                        new []{ new Permission(1, Deny) { DisplayName = "Permission 1"} }
                    }
                };
            values.GlobalPermissions.Returns(fakeICompliantValues);

            var sut = new SecurityReportProcessor();
            SecurityReport result = sut.Evaluate(data, values);
            
            Assert.NotNull(result);
            Assert.NotNull(result.GlobalPermissions);

            var compliantGroupOne = result.GlobalPermissions.Single(x => x.ApplicationGroupName == "group1");
            compliantGroupOne.Permissions.ShouldContain(x => x.ActualPermissionId == Allow && x.ShouldBePermissionId == Deny && x.PermissionBit == 1);

        }
        private static IDictionary<string, IEnumerable<Permission>> CreateFakeISecurityData()
        {
            return new Dictionary<string, IEnumerable<Permission>>
            {
                {
                    "group1",
                    new []
                    {
                        new Permission(1, Allow) { DisplayName = "Permission 1"},
                        new Permission(2, Deny) { DisplayName = "Permission 2"}
                    }
                },
                {
                    "group2",
                    new []
                    {
                        new Permission(1, NotSet) { DisplayName = "Permission 1"}
                    }
                }
            };
        }

        private static IDictionary<string, IEnumerable<Permission>> CreateFakeICompliantData()
        {
            return new Dictionary<string, IEnumerable<Permission>>
            {
                {
                    "compliantGroup1",
                    new []
                    {
                        new Permission(1, Allow) { DisplayName = "Permission 1"},
                        new Permission(2, Deny) { DisplayName = "Permission 2"}
                    }
                }
            };
        }
    }
}