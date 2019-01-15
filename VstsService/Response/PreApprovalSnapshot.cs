using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class PreApprovalSnapshot
    {
        public ApprovalOptions ApprovalOptions { get; set; }
        public IEnumerable<Approval> Approvals { get; set; }
    }
}