using System.Collections.Generic;
using Common;

namespace SecurePipelineScan.Rules
{
    public class ShouldBeData : ICompliantValues
    {
        public ShouldBeData(string project)
        {
            this.ProjectName = project;
        }

        public string ProjectName { get; set; }

        public IDictionary<string, IEnumerable<Permission>> GlobalPermissions { get; set; }
    }
}