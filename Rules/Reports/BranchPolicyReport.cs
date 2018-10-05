using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService.Response;

namespace Rules.Reports
{
    public class BranchPolicyReport : ScanReport
    {
        public MinimumNumberOfReviewersPolicy BranchPolicy { get; set; }
    }
}