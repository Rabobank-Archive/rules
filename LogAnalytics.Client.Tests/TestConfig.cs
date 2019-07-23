using Microsoft.Extensions.Configuration;

namespace LogAnalytics.Client.Tests
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

            TenantId = configuration["tenantId"];
            ClientId = configuration["clientId"];
            ClientSecret = configuration["clientSecret"];
            Key = configuration["key"];
            Workspace = configuration["workspace"];
        }

        public string ClientSecret { get; }

        public string ClientId { get; }

        public string TenantId { get; }
        public string Key { get; }
        public string Workspace { get; }
    }
}