using SecurePipelineScan.Rules.Reports;

namespace Rules.Reports
{
    public class SecurityReport
    {
        public string Project { get; set; }
        public bool ProjectIsSecure { get; set; }
        
        public bool ApplicationGroupContainsProductionEnvironmentOwner { get; set; }
        public bool ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup { get; set; }
        
        public RepositoryRights RepositoryRightsProjectAdmin { get; set; }
        
        public BuildRights BuildRightsProjectAdmin { get; set; }
        public BuildRights BuildRightsBuildAdmin { get; set; }
    }
}