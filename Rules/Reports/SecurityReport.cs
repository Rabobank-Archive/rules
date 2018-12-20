using SecurePipelineScan.Rules.Reports;

namespace Rules.Reports
{
    public class SecurityReport
    {
        public string Project { get; set; }
        public bool ProjectIsSecure { get; set; }
        
        public bool ApplicationGroupContainsProductionEnvironmentOwner { get; set; }
  
        public bool ProjectAdminHasNoPermissionToDeleteRepositorySet { get; set; }
        public bool ProjectAdminHasNoPermissionToDeleteRepositories { get; set; }
        
        public bool ProjectAdminHasNoPermissionToManagePermissionsRepositorySet { get; set; }
        public bool ProjectAdminHasNoPermissionToManagePermissionsRepositories { get; set; }
        
        
    }
}