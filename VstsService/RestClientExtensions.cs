using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using SecurePipelineScan.VstsService.Converters;

namespace SecurePipelineScan.VstsService
{
    public static class RestClientExtensions
    {

        /// <summary>
        /// https://bytefish.de/blog/restsharp_custom_json_serializer/#using-the-custom-deserializer-for-incoming-responses
        /// </summary>
        public static IRestClient SetupSerializer(this IRestClient client)
        {
            var serializer = new NewtonsoftJsonSerializer(new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = {new PolicyConverter()}
            });

            client.AddHandler("application/json", serializer);

            client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            return client;
        }
    }
}