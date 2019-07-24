using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class ProjectProperties
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
    }
}