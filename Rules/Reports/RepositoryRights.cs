namespace Rules.Reports
{
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