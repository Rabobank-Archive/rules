using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SecurePipelineScan.Rules.Security.Cmdb.Model
{
    [ExcludeFromCodeCoverage]
    public class GetCiResponse
    {
        [JsonProperty(PropertyName = "content")]
        public IEnumerable<CiContentItem> Content { get; set; }
    }
}
