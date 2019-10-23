using AutoFixture;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class PipelineHasAtLeastOneStageWithApprovalTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public PipelineHasAtLeastOneStageWithApprovalTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public async Task EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, "1"))
                .ConfigureAwait(false);

            //Act
            var rule = new PipelineHasAtLeastOneStageWithApproval();
            (await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline)).ShouldBeTrue();
        }

        [Theory]
        [InlineData(false,true)]
        [InlineData(true,false)]
        public async Task GivenReleaseCreatorCanBeApprover_ShouldEvaluate(bool releaseCreatorCanBeApprover, bool compliant)
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize<ApprovalOptions>(ctx =>
                ctx.With(a => a.ReleaseCreatorCanBeApprover, releaseCreatorCanBeApprover));
            var releasePipeline = fixture.Create<ReleaseDefinition>();
            
            //Act
            var rule = new PipelineHasAtLeastOneStageWithApproval();
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);
            
            //Assert
            result.ShouldBe(compliant);

        }

        [Fact]
        public async Task GivenNoApprovers_ShouldBeNonCompliant()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize<Approval>(ctx =>
                ctx.With(a => a.Approver, (Identity)null));
            var releasePipeline = fixture.Create<ReleaseDefinition>();
            
            //Act
            var rule = new PipelineHasAtLeastOneStageWithApproval();
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);
            
            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenNoApprovalOptions_ShouldBeNonCompliant()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize<PreDeployApprovals>(ctx =>
                ctx.With(a => a.ApprovalOptions, (ApprovalOptions)null));
            var releasePipeline = fixture.Create<ReleaseDefinition>();

            //Act
            var rule = new PipelineHasAtLeastOneStageWithApproval();
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);
            
            //Assert
            result.ShouldBe(false);
        }
    }
}