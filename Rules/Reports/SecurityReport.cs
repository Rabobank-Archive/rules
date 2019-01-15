using System;
using SecurePipelineScan.Rules.Reports;

namespace Rules.Reports
{
    public class SecurityReport
    {
        public SecurityReport(DateTime date)
        {
            Date = date;
        }

        public string Project { get; set; }
        
        public bool ApplicationGroupContainsProductionEnvironmentOwner { get; set; }
        public bool ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup { get; set; }
        public GlobalRights TeamRabobankProjectAdministrators { get; set; }

        public RepositoryRights RepositoryRightsProjectAdmin { get; set; }
        
        public BuildRights BuildRightsProjectAdmin { get; set; }
        public BuildRights BuildRightsBuildAdmin { get; set; }
        public BuildRights BuildDefinitionsRightsProjectAdmin { get; set; }
        public BuildRights BuildDefinitionsRightsBuildAdmin { get; set; }
        
        public ReleaseRights ReleaseRightsRaboProjectAdmin { get; set; }
        public ReleaseRights ReleaseRightsContributor { get; set; }
        public ProductionEnvOwnerReleaseRights ReleaseRightsProductionEnvOwner { get; set; }

        public bool ProjectIsSecure => ApplicationGroupContainsProductionEnvironmentOwner &&
                                       ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup &&
                                       TeamRabobankProjectAdministrators.GlobalRightsIsSecure &&
                                       BuildRightsBuildAdmin.BuildRightsIsSecure &&
                                       BuildRightsProjectAdmin.BuildRightsIsSecure &&
                                       BuildDefinitionsRightsProjectAdmin.BuildRightsIsSecure &&
                                       BuildDefinitionsRightsBuildAdmin.BuildRightsIsSecure &&
                                       RepositoryRightsProjectAdmin.RepositoryRightsIsSecure &&
                                       ReleaseRightsRaboProjectAdmin.IsSecure &&
                                       ReleaseRightsContributor.IsSecure &&
                                       ReleaseRightsProductionEnvOwner.IsSecure
        ;

        public DateTime Date { get; }
    }
}