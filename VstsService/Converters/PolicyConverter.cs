using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Converters
{
    public class PolicyConverter : JsonConverter<Policy>
    {
        public override void WriteJson(JsonWriter writer, Policy value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override Policy ReadJson(JsonReader reader, Type objectType, Policy existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            switch (jo["type"]["id"].Value<string>())
            {
                case "fa4e907d-c16b-4a4c-9dfa-4906e5d171dd":
                    return jo.ToObject<MinimumNumberOfReviewersPolicy>();
                case "fd2167ab-b0be-447a-8ec8-39368250530e":
                    return jo.ToObject<RequiredReviewersPolicy>();
                default:
                    return jo.ToObject<Policy>();
            }
        }
    }
}