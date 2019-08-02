using System.IO;
using AutoFixture;
using AzDoCompliancy.CustomStatus.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace AzDoCompliancy.CustomStatus.Tests
{
    public class CustomStatusConverterTests
    {
        private readonly Fixture _fixture;

        public CustomStatusConverterTests()
        {
            _fixture = new Fixture();
        }
        
        [Fact]
        public void ScanOrchestrationCustomStatusTest()
        {
            var input = File.ReadAllText(Path.Combine("CustomStatus", "ScanOrchestrationCustomStatus.json"));
            var result = JsonConvert.DeserializeObject<CustomStatusBase>(input, new CustomStatusConverter());

            result.ShouldBeOfType<ScanOrchestrationStatus>();
        }
        
        [Fact]
        public void SupervisorOrchestrationCustomStatusTest()
        {
            var input = File.ReadAllText(Path.Combine("CustomStatus", "SupervisorOrchestrationCustomStatus.json"));
            var result = JsonConvert.DeserializeObject<CustomStatusBase>(input, new CustomStatusConverter());

            result.ShouldBeOfType<SupervisorOrchestrationStatus>();
        }
        
        [Fact]
        public void OtherCustomStatusTest()
        {
            var input = File.ReadAllText(Path.Combine("CustomStatus", "OtherCustomStatus.json"));
            var result = JsonConvert.DeserializeObject<CustomStatusBase>(input, new CustomStatusConverter());

            result.ShouldBeOfType<CustomStatusBase>();
        }

        [Fact]
        public void ShouldConvertJTokenToObject()
        {
            var token = JToken.FromObject(_fixture.Create<SupervisorOrchestrationStatus>());
            
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new CustomStatusConverter());
            var obj = token.ToObject<CustomStatusBase>(serializer);

            obj.ShouldBeOfType<SupervisorOrchestrationStatus>();
        }

        [Fact]
        public void ShouldReturnNullIfCannotConvert()
        {
            const string input = "{\"some key\": \"some value\"}";
            var result = JsonConvert.DeserializeObject<CustomStatusBase>(input, new CustomStatusConverter());
            result.ShouldBeNull();
        }
        
    }
}