namespace SecurePipelineScan.Rules.Reports
{
    public class RepositoryRights
    {
        public bool HasNoPermissionToDeleteRepositorySet { get; set; }
        public bool HasNoPermissionToDeleteRepositories { get; set; }
        public bool HasNotSetToManagePermissionsRepositorySet { get; set; }
        public bool HasNotSetToManagePermissionsRepositories { get; set; }

        public bool RepositoryRightsIsSecure => HasNoPermissionToDeleteRepositories &&
                                                HasNoPermissionToDeleteRepositorySet &&
                                                HasNotSetToManagePermissionsRepositories &&
                                                HasNotSetToManagePermissionsRepositorySet;
    }
}