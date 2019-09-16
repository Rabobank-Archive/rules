using Newtonsoft.Json;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class ReleaseDefinition
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public IList<ReleaseDefinitionEnvironment> Environments { get; set; }
        public TeamProjectReference ProjectReference { get; set; }
        public IList<Artifact> Artifacts { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}