using Microsoft.Extensions.Configuration;
using RestSharp;

namespace lib.tests
{
    internal static class VstsClientFactory
    {
        public static IRestClient Create()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var token = configuration["token"];
            return new VstsClient("somecompany", token);
        }
    }
}