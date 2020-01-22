using AutoFixture;
using NSubstitute;
using SecurePipelineScan.Rules.Security.Cmdb;
using SecurePipelineScan.Rules.Security.Cmdb.Client;
using SecurePipelineScan.Rules.Security.Cmdb.Model;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ReleasePipelineHasDeploymentMethodTests : IClassFixture<TestConfig>
    {
        [Fact]
        public async Task WhenReleasePipelineHasNoDeploymentMethodsThenShouldReturnNull()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();
            var cmdbClient = new CmdbClient(null);

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod(null, cmdbClient);
            var result = await rule.EvaluateAsync("TAS", null, releasePipeline);

            //Assert
            result.ShouldBe(null);
        }

        [Fact]
        public async Task WhenReleasePipelineHasDeploymentMethodsThenShouldReturnTrue()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();
            var cmdbClient = new CmdbClient(null);

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod(null, cmdbClient);
            var result = await rule.EvaluateAsync("TAS", releasePipeline.Environments.First().Id, releasePipeline);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task WhenReleasePipelineHasNoValidDeploymentMethodsThenShouldReturnFalse()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();
            var cmdbClient = new CmdbClient(null);

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod(null, cmdbClient);
            var result = await rule.EvaluateAsync("TAS", "???", releasePipeline);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task WhenValidUserThenCanUpdateDeploymentInfo()
        {
            // Arrange
            var ci = new CiContentItem() { Device = new ConfigurationItemModel { ConfigurationItem = "TEST", AssignmentGroup = "TAS" } };
            var cmdbClient = Substitute.For<ICmdbClient>();
            cmdbClient.GetCiAsync("CI1234567").Returns(Task.FromResult(ci));
            cmdbClient.Config.Returns(new CmdbClientConfig("abc", "https://localhost", "somecompany"));

            var assignment = new AssignmentContentItem { Assignment = new Assignment { Operators = new List<string> { "x.y@somecompany.nl" } } };
            cmdbClient.GetAssignmentAsync("TAS").Returns(Task.FromResult(assignment));

            var vstsClient = Substitute.For<IVstsRestClient>();
            vstsClient.GetAsync(Arg.Any<IVstsRequest<UserEntitlement>>()).Returns(Task.FromResult(new UserEntitlement { User = new User { PrincipalName = "x.y@somecompany.nl" } }));
            var rule = new ReleasePipelineHasDeploymentMethod(vstsClient, cmdbClient);

            // Act
            dynamic data = new { User = "2dbe73bd-5f5c-6152-b980-1b9e87449188", CiIdentifier = "CI1234567" };
            await rule.ReconcileAsync("1", "2", "3", data);

            // Assert
            await cmdbClient.Received().GetCiAsync("CI1234567");
            await cmdbClient.Received().GetAssignmentAsync("TAS");
            await cmdbClient.Received().UpdateDeploymentMethodAsync("TEST", Arg.Is<ConfigurationItemModel>(c => c.DeploymentInfo.Count() == 1 && c.DeploymentInfo.First().DeploymentMethod == "Azure Devops"));
        }
    }
}