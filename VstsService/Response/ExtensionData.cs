using Newtonsoft.Json;

namespace SecurePipelineScan.VstsService.Response
{
    public class ExtensionData
    {
        public string Id { get; set; }
        
        [JsonProperty("__etag")]
        public int Etag { get; set; } = -1;

        protected ExtensionData()
        {
        }
    }
}