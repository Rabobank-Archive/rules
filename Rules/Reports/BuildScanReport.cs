using System;

namespace SecurePipelineScan.Rules.Reports
{
    public class BuildScanReport
    {
        public string Id { get; set; }
        public string Pipeline { get; set; }
        public string Project { get; set; }
        public bool ArtifactsStoredSecure { get; set; }
        public bool UsesFortify { get; set; }
        public bool UsesSonarQube { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}