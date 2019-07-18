using System;
using AzDoCompliancy.CustomStatus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DurableFunctionsAdministration.Client.CustomStatus
{
    public class CustomStatusConverter : JsonConverter<AzDoCompliancy.CustomStatus.CustomStatus>
    {
        public override void WriteJson(JsonWriter writer, AzDoCompliancy.CustomStatus.CustomStatus value, JsonSerializer serializer)
        {
            JToken.FromObject(value, serializer).WriteTo(writer);
        }

        public override AzDoCompliancy.CustomStatus.CustomStatus ReadJson(JsonReader reader, Type objectType, AzDoCompliancy.CustomStatus.CustomStatus existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            switch (jo["typeId"].Value<string>())
            {
                case TypeIds.ScanOrchestrationStatusId:
                    return jo.ToObject<ScanOrchestrationStatus>();
                case TypeIds.SupervisorOrchestrationStatusId:
                    return jo.ToObject<SupervisorOrchestrationStatus>();
                default:
                    return jo.ToObject<AzDoCompliancy.CustomStatus.CustomStatus>();
            }
        }
    }
}