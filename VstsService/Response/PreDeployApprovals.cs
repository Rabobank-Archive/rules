namespace SecurePipelineScan.VstsService.Response
{
    public class PreDeployApprovals
    {
        public ApprovalOptions ApprovalOptions { get; set; }
        public Approval[] Approvals { get; set; }
    }
}