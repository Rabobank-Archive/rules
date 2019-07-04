using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class PipelineHasAtLeastOneStageWithApprovalTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client = Substitute.For<IVstsRestClient>();

        public PipelineHasAtLeastOneStageWithApprovalTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public async Task EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);

            //Act
            var rule = new PipelineHasAtLeastOneStageWithApproval(client);
            await rule.Evaluate(_config.Project, "1");
        }

        [Theory]
        [InlineData(false,true)]
        [InlineData(true,false)]
        public async Task GivenStageAndApproval_Evaluate(bool releaseCreatorCanBeApprover, bool compliant)
        {
            //Arrange
            SetupClient(_client, releaseCreatorCanBeApprover);

            //Act
            var rule = new PipelineHasAtLeastOneStageWithApproval(_client);
            var result = await rule.Evaluate(_config.Project, "1");
            
            //Assert
            result.ShouldBe(compliant);

        }
        
        private static void SetupClient(IVstsRestClient client, bool releaseCreatorCanBeApprover)
        {
            client
                .GetAsync(Arg.Any<IVstsRequest<ReleaseDefinition>>())
                .Returns(new ReleaseDefinition
                {
                    Environments = new List<ReleaseDefinitionEnvironment>
                    {
                        new ReleaseDefinitionEnvironment
                        {
                            Name = "Stage 1",
                            PreDeployApprovals = new PreDeployApprovals
                            {
                                ApprovalOptions = new ApprovalOptions
                                {
                                    ReleaseCreatorCanBeApprover = true
                                }
                            }
                        },
                        new ReleaseDefinitionEnvironment() 
                        {
                            Name = "Stage 2",
                            PreDeployApprovals = new PreDeployApprovals
                            {
                                ApprovalOptions = new ApprovalOptions
                                {
                                    ReleaseCreatorCanBeApprover = releaseCreatorCanBeApprover
                                }
                            }
                        }
                    }
                });
        }
    }
}