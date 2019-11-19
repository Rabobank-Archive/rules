using System;
using System.Threading.Tasks;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class PipelineEvaluatorFactoryTests
    {
        [Theory]
        [InlineData(1, typeof(GuiPipelineEvaluator))]
        [InlineData(2, typeof(YamlPipelineEvaluator))]
        public void CanCreatePipelineEvaluator(int buildProcessType, Type evaluatorType)
        {
            // arrange
            var factory = new PipelineEvaluatorFactory(null);
            var buildDefinition = new BuildDefinition { Process = new BuildProcess { Type = buildProcessType } };

            // act 
            var evaluator = factory.Create(buildDefinition);

            // assert
            evaluator.ShouldBeOfType(evaluatorType);
        }
    }
}