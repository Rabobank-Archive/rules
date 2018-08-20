using System;
using Xunit;

namespace VstsService.Tests
{
    public class UnitTest1
    {
        const string encodedPath = "<>";

        [Fact]
        public void Test1()
        {
            var scanner = new VstsScanner($"Basic {encodedPath}", "somecompany");
            scanner.GetReleasesByTeam();

            
        }
    }
}
