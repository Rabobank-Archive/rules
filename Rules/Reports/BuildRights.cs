namespace Rules.Reports
{
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
}