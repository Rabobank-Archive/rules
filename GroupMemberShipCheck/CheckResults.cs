using System.Collections.Generic;
using System.Collections.Specialized;

namespace SecurePipelineScan.GroupMemberShipCheck
{
    internal class CheckResults
    {
        public List<string> okProjectNames { get; internal set; }
        public List<string> notOkProjectNames { get; internal set; }
        public NameValueCollection notOkProjectMembers { get; internal set; }
    }
}