using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class ServiceEndpoint
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Uri Url { get; set; }
    }
}

