using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ShouldBeDataFetcherTests
    {
        [Fact]
        public void FetchGlobalShouldBePermissionsWithFileShouldReturnGlobals()
        {
            ShouldBeDataFetcher sut = new ShouldBeDataFetcher();

            var projectName = "SoxPipeline";
            var result = sut.FetchGlobalShouldBePermissions(projectName, () => ShouldBeDataFetcher.ReadFileForProject(projectName));

            Assert.NotNull(result);
            result.ShouldBeAssignableTo<ShouldBeData>();
        }

        [Fact]
        public void FetchGlobalShouldBePermissionsShouldReturnCorrectPermissions()
        {
            ShouldBeDataFetcher sut = new ShouldBeDataFetcher();
            var projectName = "SoxPipeline";

            var result = sut.FetchGlobalShouldBePermissions(projectName, () => ShouldBeDataFetcher.ReadFileForProject(projectName));

            var globalPermissions = result.GlobalPermissions.SingleOrDefault(x => x.Key == "shouldBeGroupA");

            globalPermissions.Value.ShouldContain(x => x.PermissionBit == 111);
            globalPermissions.Value.ShouldAllBe(x => x.PermissionBit > 0);
        }

        [Fact]
        public void CreateJson_ShouldReturnShouldBeData()
        {
            var json = "{\"ProjectName\":\"Test\",\"ProjectId\":\"123\",\"GlobalPermissions\":[{\"ApplicationGroupName\":\"shouldBeGroupA\",\"Permissions\":[{\"PermissionId\":1,\"PermissionBit\":45},{\"PermissionId\":2,\"PermissionBit\":99}]}]}";
            ShouldBeDataFetcher sut = new ShouldBeDataFetcher();
            var projectName = "Test";

            var fetchSecurityShouldBePermissions = sut.FetchGlobalShouldBePermissions(projectName, () => json);

            var soxPipelineData = fetchSecurityShouldBePermissions.GlobalPermissions.SingleOrDefault(x => x.Key == "shouldBeGroupA");

            soxPipelineData.Value.ShouldContain(x => x.PermissionBit == 99);

            Assert.NotNull(soxPipelineData.Value);
        }

        [Fact]
        public void CreateJson_NoFileShouldReturnDefaultShouldBeSettings()
        {
            ShouldBeDataFetcher sut = new ShouldBeDataFetcher();
            var projectName = "Test";

            var fetchSecurityShouldBePermissions = sut.FetchGlobalShouldBePermissions(projectName,
                () => throw new ArgumentNullException("file not found"));

            fetchSecurityShouldBePermissions.ProjectName.ShouldBe("Test");
        }
    }
}