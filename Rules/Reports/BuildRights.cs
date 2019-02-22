namespace SecurePipelineScan.Rules.Reports
{
    public abstract class BuildRights
    {
        public bool HasNoPermissionsToAdministerBuildPermissions { get; set; }
        public bool HasNoPermissionsToDeleteBuildDefinition { get; set; }
        public bool HasNoPermissionsToDeleteBuilds { get; set; }
        public bool HasNoPermissionsToDestroyBuilds { get; set; }
        public bool HasNotSetToDeleteBuildDefinition { get; set; }
        public bool HasNotSetToDeleteBuilds { get; set; }
        public bool HasNotSetToDestroyBuilds { get; set; }


        public abstract bool IsSecure { get;}
    }
}