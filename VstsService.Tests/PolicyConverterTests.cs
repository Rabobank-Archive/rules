using System.IO;
using Newtonsoft.Json;
using SecurePipelineScan.VstsService.Converters;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace VstsService.Tests
{
    public class PolicyConverterTests
    {
        [Fact]
        public void MinimumNumberOfReviewersPolicyConvertTest()
        {
            var input = File.ReadAllText(Path.Combine("Policies", "MinimumNumberOfReviewers.json"));
            var result = JsonConvert.DeserializeObject<Policy>(input, new PolicyConverter());

            result.ShouldBeOfType<MinimumNumberOfReviewersPolicy>();
        }
        
        [Fact]
        public void RequiredReviewersPolicyConvertTest()
        {
            var input = File.ReadAllText(Path.Combine("Policies", "RequiredReviewers.json"));
            var result = JsonConvert.DeserializeObject<Policy>(input, new PolicyConverter());

            result.ShouldBeOfType<RequiredReviewersPolicy>();
        }
    }
}