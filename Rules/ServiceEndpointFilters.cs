using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SecurePipelineScan.Rules
{
    internal static class ServiceEndpointFilters
    {
        public static IEnumerable<JToken> ProductionEndpoints(this JToken endpoints)
        {
            return endpoints
                .SelectTokens("value[*]")
                .FilterTestEndpoints()
                .FilterAzureEndpoints();
        }
        
        private static IEnumerable<JToken> FilterTestEndpoints(this IEnumerable<JToken> endpoints)
        {
            return endpoints.Where(e => !((string)e["url"]).Contains("test"));
        }

        private static readonly string[] AzureProductionSubscriptions = 
            { "a2439340-3e5e-4290-bc80-89065170bc86", "f13f81f8-7578-4ca8-83f3-0a845fad3cb5" };
        
        private static IEnumerable<JToken> FilterAzureEndpoints(this IEnumerable<JToken> endpoints)
        {
            return endpoints
                .Where(e => (string)e["type"] != "azurerm" || AzureProductionSubscriptions.Contains((string) e.SelectToken("data.subscriptionId")));
        }
    }
}