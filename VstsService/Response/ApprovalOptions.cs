namespace SecurePipelineScan.VstsService.Response
{
    public class ApprovalOptions
    {
        public int? RequiredApproverCount { get; set; }
        public bool ReleaseCreatorCanBeApprover { get; set; }
    }
}