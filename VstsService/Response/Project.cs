using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class Project
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string Description { get; set; }

        public Uri Url { get; set; }
    }
}