namespace SecurePipelineScan.VstsService.Response
{
    public class RequiredReviewersPolicy
    {
        public string Id { get; internal set; }

        public bool? IsEnabled { get; set; }
        public bool? IsBlocking { get; set; }
        public bool? IsDeleted { get; set; }

        public RequiredReviewersPolicySettings Settings { get; set; }
    }
}