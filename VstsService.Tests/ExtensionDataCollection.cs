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
            var result = _client.Put(Requests.ExtensionDataCollections.ExtensionData("riezebosch","my-first-extension", "DevOps Demo","geert",new TestObject() {Name="Geert", Value="bla" }));
            result.Id.ShouldBe("geert");
        }

        [Fact]
        [Trait("category", "integration")]
        public void GetDataFromExtensionDataCollection()
        {
            var result = _client.Get(Requests.ExtensionDataCollections.ExtensionData("riezebosch", "my-first-extension", "DevOps Demo", "geert"));
            result.Id.ShouldBe("geert");
        }

        internal class TestObject
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

    }
}