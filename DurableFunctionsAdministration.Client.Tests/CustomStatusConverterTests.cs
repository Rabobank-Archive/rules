using System.IO;
using AzDoCompliancy.CustomStatus;
using DurableFunctionsAdministration.Client.CustomStatus;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace DurableFunctionsAdministration.Client.Tests
{
    public class CustomStatusConverterTests
    {
        [Fact]
        public void ScanOrchestrationCustomStatusTest()
        {
            var input = File.ReadAllText(Path.Combine("CustomStatus", "ScanOrchestrationCustomStatus.json"));
            var result = JsonConvert.DeserializeObject<CustomStatusBase>(input, new CustomStatusConverter());

            result.ShouldBeOfType<ScanOrchestrationStatusBase>();
        }
        
        [Fact]
        public void SupervisorOrchestrationCustomStatusTest()
        {
            var input = File.ReadAllText(Path.Combine("CustomStatus", "SupervisorOrchestrationCustomStatus.json"));
            var result = JsonConvert.DeserializeObject<CustomStatusBase>(input, new CustomStatusConverter());

            result.ShouldBeOfType<SupervisorOrchestrationStatusBase>();
        }
        
        [Fact]
        public void OtherCustomStatusTest()
        {
            var input = File.ReadAllText(Path.Combine("CustomStatus", "OtherCustomStatus.json"));
            var result = JsonConvert.DeserializeObject<CustomStatusBase>(input, new CustomStatusConverter());

            result.ShouldBeOfType<CustomStatusBase>();
        }
        
    }
}