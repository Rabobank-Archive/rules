using ExpectedObjects;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace VstsService.Tests
{
    public class MultipleTests
    {
        [Fact]
        public void InitializeWithItems()
        {
            var multiple = new Multiple<string>("one", "two", "three");
            
            new[] {"one", "two", "three"}.ToExpectedObject().ShouldEqual(multiple.Value);
            multiple.Count.ShouldBe(3);
        }
    }
}