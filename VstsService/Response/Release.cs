using System.Collections.Generic;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.VstsService.Response
{
    public class Release
    {
        public string Id { get; set; }
        public IEnumerable<Environment> Environments { get; set; }
        public IEnumerable<ArtifactReference> Artifacts { get; set; }
    }
}