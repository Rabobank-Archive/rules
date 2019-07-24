using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Converters
{
    public class PolicyConverter : JsonConverter<Policy>
    {
        public override void WriteJson(JsonWriter writer, Policy value, JsonSerializer serializer)
        {
            if (value != null)
            {
                switch (value)
                {
                    case MinimumNumberOfReviewersPolicy _:
                        value.Type = new PolicyType { Id = new Guid("fa4e907d-c16b-4a4c-9dfa-4906e5d171dd") };
                        break;
                    case RequiredReviewersPolicy _:
                        value.Type = new PolicyType { Id = new Guid("fd2167ab-b0be-447a-8ec8-39368250530e") };
                        break;
                    default:
                        throw new ArgumentException("Unknown policy type");
                }
            }
            JToken.FromObject(value,new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver()}).WriteTo(writer);
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