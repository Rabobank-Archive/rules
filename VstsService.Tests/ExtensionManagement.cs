using Shouldly;
using System.Linq;
using AutoFixture;
using ExpectedObjects;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Xunit;

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
        public void PutExtensionDataReturnsId()
        {
            // Arrange
            var data = _fixture.Create<TestObject>();

            // Act
            var result = _client.Put(Requests.ExtensionManagement.ExtensionData<TestObject>(
                "ms", 
                "vss-analytics",
                "DevOps Demo"), data);
            
            // Assert
            result
                .Id
                .ShouldBe(data.Id);
        }


        [Fact]
        public void GetReturnsDataFromPut()
        {
            // Arrange
            var data = _fixture.Create<TestObject>();
            _client.Put(Requests.ExtensionManagement.ExtensionData<TestObject>(
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
            var result =  _client.Get(Requests.ExtensionManagement.ExtensionData<TestObject>(
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