using System.Collections.Generic;

namespace lib.Response
{
    public class Environment
    {
        public string Id { get; set; }
        public IEnumerable<PreDeployApproval> PreDeployApprovals { get; set; }
    }
}