using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Reports
{
    public class ReleaseReport : ScanReport
    {
        public Response.Release Release { get; set; }
        public bool Result { get; set; }
    }
}