using Microsoft.Extensions.Configuration;

namespace SecurePipelineScan.Rules.Tests
{
    public class TestConfig
    {
        public TestConfig()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.user.json", true)
                .AddEnvironmentVariables()
                .Build();

            Token = configuration["token"];
            StageId = configuration["stageId"];
            Project = configuration["project"];
            Organization = configuration["organization"];
            ExtensionName = configuration["extensionname"];
        }

        public string Token { get; }
        public string Project { get; }
        public string StageId { get; set; }
        public string Organization { get; }
        public string ExtensionName { get; }
    }
}