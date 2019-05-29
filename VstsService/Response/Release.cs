using System;
using System.Collections.Generic;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.VstsService.Response
{
    public class Release
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ReleaseDefinition ReleaseDefinition { get; set; }
        public IEnumerable<Environment> Environments { get; set; }
        public IEnumerable<ArtifactReference> Artifacts { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}