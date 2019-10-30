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
            BuildDefinitionId = configuration["buildDefinitionId"] ?? "2";
            BuildId = configuration["buildId"] ?? "4";
            RepositoryId = configuration["repositoryId"] ?? "6435e3f0-15b7-4302-814d-4ab586e61f8b";
            GitItemFilePath = configuration["gitItemFilePath"] ?? "/azure-pipelines.yml";

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
        public string BuildDefinitionId { get; }
        public string RepositoryId { get; }
        public string GitItemFilePath { get; }
    }
}