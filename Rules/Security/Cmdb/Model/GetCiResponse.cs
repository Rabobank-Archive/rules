using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SecurePipelineScan.Rules.Security.Cmdb.Model
{
    [ExcludeFromCodeCoverage]
    public class GetCiResponse
    {
        public IEnumerable<CiContentItem> Content { get; set; }
    }
}
