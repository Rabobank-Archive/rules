using System;

namespace SecurePipelineScan.Rules.Reports
{
    public class ReleaseDeploymentCompletedReport
    {
        public string Project { get; set; }
        public string Pipeline { get; set; }
        public string Release { get; set; }
        public string Environment { get; set; }
        public string ReleaseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? HasApprovalOptions { get; set; }
        public bool? HasBranchFilterForAllArtifacts { get; set; }
        public bool? AllArtifactsAreFromBuild { get; set; }
        public bool? UsesManagedAgentsOnly { get; set; }
        public string SM9ChangeId { get; set; }
    }
}