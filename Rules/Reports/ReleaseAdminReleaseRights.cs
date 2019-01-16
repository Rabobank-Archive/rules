namespace SecurePipelineScan.Rules.Reports
{  
        public class ReleaseAdminReleaseRights : ReleaseRights
        {
            public override bool IsSecure =>
                HasNoPermissionsToAdministerReleasePermissions &&
                HasNoPermissionToDeleteReleasePipeline &&
                HasNoPermissionToDeleteReleases &&
                HasNoPermissionToManageReleaseApprovers &&
                HasNoPermissionToDeleteReleaseStage;
        }
}