using Microsoft.Extensions.Configuration;

namespace DurableFunctionsAdministration.Client.Tests
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

            Code = configuration["code"];
            TaskHub = configuration["taskHub"];
            BaseUri = configuration["baseUri"];
        }

        public string Code { get; }
        public string TaskHub { get; }
        public string BaseUri { get; }
    }
}