using Newtonsoft.Json;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class ReleaseDefinition
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<ReleaseDefinitionEnvironment> Environments { get; set; }


        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}