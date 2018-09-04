using lib.Vsts;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace lib.tests
{
    internal static class VstsClientFactory
    {
        public static IVstsClient Create()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var token = configuration["token"];
            return new VstsClient("somecompany", token);
        }
    }
}