using lib.Vsts;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace lib.tests
{
    internal static class VstsClientFactory
    {
        public static IVstsRestClient Create()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.secrets.json")
                .Build();

            var token = configuration["token"];
            return new VstsRestClient("somecompany", token);
        }
    }
}