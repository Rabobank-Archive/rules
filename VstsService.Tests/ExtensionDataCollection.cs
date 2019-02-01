using Shouldly;
using System.Linq;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class ExtensionDataCollection : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private IVstsRestClient _client;

        public ExtensionDataCollection(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public void AddDataToExtensionDataCollection()
        {
            var putResult = _client.Put(Requests.ExtensionDataCollections.ExtensionData("ms", "vss-analytics", "DevOps Demo","geert",new TestObject() {Name="Geert", Value="bla" }));
            putResult.Id.ShouldBe("geert");

            var getResult = _client.Get(Requests.ExtensionDataCollections.ExtensionData("ms", "vss-analytics", "DevOps Demo", "geert"));
            putResult.Id.ShouldBe("geert");
        }

        internal class TestObject
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

    }
}