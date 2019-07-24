using System;
using AzDoCompliancy.CustomStatus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DurableFunctionsAdministration.Client.CustomStatus
{
    public class CustomStatusConverter : JsonConverter<AzDoCompliancy.CustomStatus.ICustomStatus>
    {
        public override void WriteJson(JsonWriter writer, AzDoCompliancy.CustomStatus.ICustomStatus value, JsonSerializer serializer)
        {
            JToken.FromObject(value, serializer).WriteTo(writer);
        }

        public override AzDoCompliancy.CustomStatus.ICustomStatus ReadJson(JsonReader reader, Type objectType, AzDoCompliancy.CustomStatus.ICustomStatus existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            if (jo.TryGetValue("TypeId", StringComparison.CurrentCulture, out var typeId))
            {
                switch (typeId.Value<string>())
                {
                    case TypeIds.ScanOrchestrationStatusId:
                        return jo.ToObject<ScanOrchestrationStatus>();
                    case TypeIds.SupervisorOrchestrationStatusId:
                        return jo.ToObject<SupervisorOrchestrationStatus>();
                    default:
                        return jo.ToObject<AzDoCompliancy.CustomStatus.ICustomStatus>();
                }
            }

            return null;
        }
    }
}