namespace SecurePipelineScan.Rules.Reports
{
    public class GlobalRights
    {
        public bool HasNoPermissionToDeleteTeamProject { get; set; }
        public bool HasNoPermissionToPermanentlyDeleteWorkitems { get; set; }
        public bool HasNoPermissionToManageProjectProperties { get; set; }

        public bool GlobalRightsIsSecure => HasNoPermissionToDeleteTeamProject &&
                                            HasNoPermissionToPermanentlyDeleteWorkitems &&
                                            HasNoPermissionToManageProjectProperties;
    }
}