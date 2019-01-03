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

    public class BuildRights
    {
        public bool HasNoPermissionsToAdministerBuildPermissions { get; set; }
        public bool HasNoPermissionsToDeleteBuildDefinition { get; set; }
        public bool HasNoPermissionsToDeleteBuilds { get; set; }
        public bool HasNoPermissionsToDeDestroyBuilds { get; set; }

        public bool BuildRightsIsSecure => HasNoPermissionsToDeleteBuilds && 
                                           HasNoPermissionsToDeleteBuildDefinition &&
                                           HasNoPermissionsToDeDestroyBuilds && 
                                           HasNoPermissionsToAdministerBuildPermissions;
    }

    public class RepositoryRights
    {
        public bool HasNoPermissionToDeleteRepositorySet { get; set; }
        public bool HasNoPermissionToDeleteRepositories { get; set; }
        public bool HasNoPermissionToManagePermissionsRepositorySet { get; set; }
        public bool HasNoPermissionToManagePermissionsRepositories { get; set; }

        public bool RepositoryRightsIsSecure => HasNoPermissionToDeleteRepositories &&
                                                HasNoPermissionToDeleteRepositorySet &&
                                                HasNoPermissionToManagePermissionsRepositories &&
                                                HasNoPermissionToManagePermissionsRepositorySet;
    }
}