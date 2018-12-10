using System.Linq;
using ExpectedObjects;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
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

        [Fact]
        public void TreatMultipleAsEnumerable()
        {
            var multiple = new Multiple<string>("one", "two", "three");
            new[] {"one", "two", "three"}.ToExpectedObject().ShouldEqual(multiple.ToArray());
        }
    }
}