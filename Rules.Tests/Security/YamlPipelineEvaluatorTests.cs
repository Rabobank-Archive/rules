using SecurePipelineScan.Rules.Security;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class YamlPipelineEvaluatorTests
    {
        [Theory]
        [InlineData("a.b.c@1", "c", true)]
        [InlineData("@1", "", true)]
        [InlineData("", "", true)]
        [InlineData("a.b.c", "c", true)]
        [InlineData("c", "c", true)]
        [InlineData("a.b.c@1", "a", false)]
        public void ContainsTaskName_ShouldReturnTaskNameWithoutPrefixAndVersion(string taskName,
            string expectedTaskName, bool expectedResult)
        {
            Assert.Equal(expectedResult, YamlPipelineEvaluator.ContainsTaskName(taskName, expectedTaskName));
        }
    }
}