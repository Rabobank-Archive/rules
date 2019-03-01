using Common;
using Xunit;

namespace SecurePipelineScan.Common.Tests
{
    public class EnumExtensionsTest
    {
        [Fact]
        public void GetDisplayNameWithNullValueAsInputShouldReturnEmptyString()
        {
            Assert.Empty(EnumExtensions.GetDisplayName(null));
        }
    }
}