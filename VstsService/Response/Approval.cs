namespace SecurePipelineScan.VstsService.Response
{
    public class Approval
    {
        public bool IsAutomated { get; set; }
        public Identity Approver { get; set; }
    }
}