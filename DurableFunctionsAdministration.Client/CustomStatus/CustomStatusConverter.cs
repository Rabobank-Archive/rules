using System;
using AzDoCompliancy.CustomStatus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DurableFunctionsAdministration.Client.CustomStatus
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
            var jo = JObject.Load(reader);

            if (jo.TryGetValue("TypeId", StringComparison.CurrentCulture, out var typeId))
            {
                switch (typeId.Value<string>())
                {
                    case TypeIds.ScanOrchestrationStatusId:
                        return jo.ToObject<ScanOrchestrationStatusBase>();
                    case TypeIds.SupervisorOrchestrationStatusId:
                        return jo.ToObject<SupervisorOrchestrationStatusBase>();
                    default:
                        return jo.ToObject<CustomStatusBase>();
                }
            }

            return null;
        }
    }
}