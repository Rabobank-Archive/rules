namespace SecurePipelineScan.VstsService.Response
{
    public class MinimumNumberOfReviewersPolicy
    {
        public string Id { get; internal set; }

        public bool? IsEnabled { get; set; }
        public bool? IsBlocking { get; set; }
        public bool? IsDeleted { get; set; }

        public MinimumNumberOfReviewersPolicySettings Settings { get; set; }
    }
}