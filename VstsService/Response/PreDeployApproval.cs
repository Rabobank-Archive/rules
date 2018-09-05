using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class PreDeployApproval
    {
        public string Status { get; set; }
        public string ApprovalType { get; set; }
        public bool IsAutomated { get; set; }
        public Identity ApprovedBy { get; set; }
    }
}