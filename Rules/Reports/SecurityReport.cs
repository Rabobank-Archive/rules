using SecurePipelineScan.Rules.Reports;

namespace Rules.Reports
{
    public class SecurityReport
    {
        public string Project { get; set; }
        public bool ProjectIsSecure { get; set; }
        
        public bool ApplicationGroupContainsProductionEnvironmentOwner { get; set; }
        public bool ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup { get; set; }

        public bool ProjectAdminHasNoPermissionToDeleteRepositorySet { get; set; }
        public bool ProjectAdminHasNoPermissionToDeleteRepositories { get; set; }
        
        public bool ProjectAdminHasNoPermissionToManagePermissionsRepositorySet { get; set; }
        public bool ProjectAdminHasNoPermissionToManagePermissionsRepositories { get; set; }
        
        public bool ProjectAdminHasNoPermissionsToAdministerBuildPermissions { get; set; }
        public bool ProjectAdminHasNoPermissionsToDeleteBuildDefinition { get; set; }
        public bool ProjectAdminHasNoPermissionsToDeleteBuilds { get; set; }
        public bool ProjectAdminHasNoPermissionsToDeDestroyBuilds { get; set; }

        public bool BuildAdminHasNoPermissionsToAdministerBuildPermissions { get; set; }
        public bool BuildAdminHasNoPermissionsToDeleteBuildDefinition { get; set; }
        public bool BuildAdminHasNoPermissionsToDeleteBuilds { get; set; }
        public bool BuildAdminHasNoPermissionsToDeDestroyBuilds { get; set; }

    }
}