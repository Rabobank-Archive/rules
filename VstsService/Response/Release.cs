using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class Release
    {
        public string Id { get; set; }
        public IEnumerable<Environment> Environments { get; set; }
    }
}