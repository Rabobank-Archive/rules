using Newtonsoft.Json;

namespace SecurePipelineScan.VstsService.Response
{
    public class Owner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "_links")]
        public Links Links { get; set; }
    }
}