using System.Collections.Generic;
using Common;

namespace SecurePipelineScan.Rules
{
    public class SecurityData : ISecurityData
    {
        
        public SecurityData(string project)
        {
            this.ProjectName = project;
        }

        public string ProjectName { get; set; }

        public IDictionary<string, IEnumerable<Permission>> GlobalPermissions { get; set; }
    }
}