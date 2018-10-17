using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Reports
{
    public class ReleaseReport : EndpointReport
    {
        public Response.Release Release { get; set; }
        public bool Result { get; set; }
    }
}