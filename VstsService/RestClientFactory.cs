using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using SecurePipelineScan.VstsService.Converters;

namespace SecurePipelineScan.VstsService
{
    public class RestClientFactory : IRestClientFactory
    {

        /// <summary>
        /// https://bytefish.de/blog/restsharp_custom_json_serializer/#using-the-custom-deserializer-for-incoming-responses
        /// </summary>
        private static IRestClient Configure(IRestClient client)
        {
            client.AddHandler("application/json", () => new NewtonsoftJsonSerializer(new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = {new PolicyConverter()}
            }));

            client.AddHandler("text/json", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/x-json", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/javascript", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("*+json",() =>  NewtonsoftJsonSerializer.Default);

            return client;
        }
        
        private readonly ConcurrentDictionary<Uri, IRestClient> _cache = new ConcurrentDictionary<Uri, IRestClient>();

        public IRestClient Create(Uri baseUri)
        {
            return _cache.GetOrAdd(baseUri, x => Configure(new RestClient(x)));
        }
    }
}