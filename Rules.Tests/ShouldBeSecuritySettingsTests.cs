using System.IO;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ShouldBeSecuritySettingsConverterTests{}
//    {
//        [Fact]
//        public void FileReadShouldNotBeOfTypeShouldBeSettings()
//        {
//            var input = File.ReadAllText(Path.Combine("ShouldBeSettings", "SecuritySettings_SoxPipeline.json"));
//            var result = JsonConvert.DeserializeObject<ShouldBeSettings>(input);
//
//            result.ShouldBeOfType<ShouldBeSettings>();
//        }
//
//        [Fact]
//        public void FileReadShouldContainGlobalRights()
//        {
//            var input = File.ReadAllText(Path.Combine("ShouldBeSettings", "SecuritySettings_SoxPipeline.json"));
//            var result = JsonConvert.DeserializeObject<ShouldBeSettings>(input);
//
//            Assert.NotNull(result.ShouldBeGlobalRights);
//        }
//
//        [Fact]
//        public void FileReadShouldContainGlobalRightsWithPermission()
//        {
//            var input = File.ReadAllText(Path.Combine("ShouldBeSettings", "SecuritySettings_SoxPipeline.json"));
//            var result = JsonConvert.DeserializeObject<ShouldBeSettings>(input);
//
//            var permission = result.ShouldBeGlobalRights.permission;
//
//            Assert.Equal(111, permission.PermissionBit);
//        }
//
//        [Fact]
//        public void ShouldBeSettingsFileShouldContainCorrectProjectName()
//        {
//            var input = File.ReadAllText(Path.Combine("ShouldBeSettings", "SecuritySettings_SoxPipeline.json"));
//            var result = JsonConvert.DeserializeObject<ShouldBeSettings>(input);
//
//            var expectedProjectName = "SoxPipeline";
//            Assert.Equal(expectedProjectName, result.ProjectName);
//        }
//    }
}