using System;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace AzureDevOps.Compliancy.Rules.Tests
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