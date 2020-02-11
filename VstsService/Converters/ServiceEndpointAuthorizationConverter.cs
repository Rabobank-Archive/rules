using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Converters
{
    internal class ServiceEndpointAuthorizationConverter : JsonConverter<IServiceEndpointAuthorization>
    {
        public override IServiceEndpointAuthorization ReadJson(JsonReader reader, Type objectType, IServiceEndpointAuthorization existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            switch (jo["scheme"].Value<string>())
            {
                case "UsernamePassword":
                    return new UsernamePassword(null, null);
                default:
                    return null;
            }

        }

        public override void WriteJson(JsonWriter writer, IServiceEndpointAuthorization value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);
    }
}