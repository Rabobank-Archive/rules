using System.Collections.Generic;
using Common;

namespace SecurePipelineScan.Rules
{
    public class ApplicationGroup
    {
        public string ApplicationGroupName { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}