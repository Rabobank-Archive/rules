using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzDoCompliancy.CustomStatus.Converter
{
    public class CustomStatusConverter : JsonConverter<CustomStatusBase>
    {
        public override void WriteJson(JsonWriter writer, CustomStatusBase value, JsonSerializer serializer)
        {
            JToken.FromObject(value, serializer).WriteTo(writer);
        }

        public override CustomStatusBase ReadJson(JsonReader reader, Type objectType, CustomStatusBase existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            
            var jo = JObject.Load(reader);

            if (jo.TryGetValue("TypeId", StringComparison.CurrentCulture, out var typeId))
            {
                return (typeId.Value<string>()) switch
                {
                    TypeIds.ScanOrchestrationStatusId => jo.ToObject<ScanOrchestrationStatus>(),
                    TypeIds.SupervisorOrchestrationStatusId => jo.ToObject<SupervisorOrchestrationStatus>(),
                    _ => jo.ToObject<CustomStatusBase>(),
                };
            }

            return null;
        }
    }
}