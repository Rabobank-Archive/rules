using System;
using Newtonsoft.Json;
using SecurePipelineScan.VstsService.Converters;

namespace SecurePipelineScan.VstsService.Response
{
    public class ServiceEndpoint
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Uri Url { get; set; }

        [JsonConverter(typeof(ServiceEndpointAuthorizationConverter))]
        public IServiceEndpointAuthorization Authorization { get; set; }
    }
}

