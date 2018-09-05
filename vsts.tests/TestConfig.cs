using Microsoft.Extensions.Configuration;

namespace Vsts.Tests
{
    public class TestConfig
    {
        public TestConfig()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.user.json", true)
                .Build();

            Token = configuration["token"];
            Project = configuration["project"];
            Organization = configuration["organization"];
        }

        public string Token { get; }
        public string Project { get; }
        public string Organization { get; }
    }
}