using System;

namespace SecurePipelineScan.Rules.Reports
{
    public class ReleaseDeploymentCompletedReport
    {
        public string Project { get; set; }
        public string Release { get; set; }
        public string Environment { get; set; }
        public string ReleaseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool UsesProductionEndpoints { get; set; }
        public bool HasApprovalOptions { get; set; }
        public string Pipeline { get; set; }
        public bool HasBranchFilterForAllArtifacts { get; set; }
    }
}