using AutoFixture;
using ExpectedObjects;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ExtensionManagement : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;
        private readonly Fixture _fixture = new Fixture();

        public ExtensionManagement(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task PutExtensionDataReturnsId()
        {
            // Arrange
            var data = _fixture.Create<TestObject>();

            // Act
            var result = await _client.PutAsync(Requests.ExtensionManagement.ExtensionData<TestObject>(
                "ms",
                "vss-analytics",
                "DevOps Demo"), data);

            // Assert
            result
                .Id
                .ShouldBe(data.Id);
        }

        [Fact]
        public async Task GetReturnsDataFromPut()
        {
            // Arrange
            var data = _fixture.Create<TestObject>();
            await _client.PutAsync(Requests.ExtensionManagement.ExtensionData<TestObject>(
                "ms",
                "vss-analytics",
                "DevOps Demo"), data);

            var expected = new
            {
                data.Id,
                data.Name,
                data.Value,
                Etag = Expect.NotDefault<int>()
            }
                .ToExpectedObject();

            // Act
            var result = await _client.GetAsync(Requests.ExtensionManagement.ExtensionData<TestObject>(
                "ms",
                "vss-analytics",
                "DevOps Demo",
                data.Id));

            // Assert
            expected.ShouldMatch(result);
        }

        private class TestObject : ExtensionData
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}