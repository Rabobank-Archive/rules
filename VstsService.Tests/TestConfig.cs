using Microsoft.Extensions.Configuration;

namespace SecurePipelineScan.VstsService.Tests
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
            Project = configuration["project"];
            Organization = configuration["organization"];
            ExpectedAgentPoolName = configuration["expectedAgentPoolName"] ?? "Default";
            ServiceEndpointId = configuration["serviceEndpointId"] ?? "975b3603-9939-4f22-a5a9-baebb39b5dad";
            ReleaseDefinitionId = configuration["releaseDefinitionId"] ?? "1";
            ReleaseDefinitionName = configuration["releaseDefinitionName"] ?? "New release pipeline";
            BuildId = configuration["buildId"] ?? "4";

            if (int.TryParse(configuration["AgentPoolId"], out int poolId))
            {
                AgentPoolId = poolId;
            }
        }

        public string Token { get; }
        public string Project { get; }
        public string Organization { get; }
        public string ExpectedAgentPoolName { get; }
        public string ServiceEndpointId { get; }
        public string ReleaseDefinitionId { get; }
        public string ReleaseDefinitionName { get; }
        public int AgentPoolId { get; } = 1;
        public string BuildId { get; }
    }
}