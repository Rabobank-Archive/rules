using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Reports
{
    public abstract class ScanReport
    {
        public ServiceEndpoint Endpoint { get; set; }
        public ServiceEndpointHistoryData Request { get; set; }
    }
}