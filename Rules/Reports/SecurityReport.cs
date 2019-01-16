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
        public BuildRights BuildRightsContributor { get; set; }
        public BuildRights BuildDefinitionsRightsProjectAdmin { get; set; }
        public BuildRights BuildDefinitionsRightsBuildAdmin { get; set; }
        public BuildRights BuildDefinitionsRightsContributor { get; set; }
        
        public ReleaseRights ReleaseRightsRaboProjectAdmin { get; set; }
        public ReleaseRights ReleaseDefinitionsRightsRaboProjectAdmin { get; set; }

        public ReleaseRights ReleaseRightsContributor { get; set; }
        public ReleaseRights ReleaseDefintionsRightsContributor { get; set; }
        
        public ReleaseRights ReleaseRightsProjectAdmin { get; set; }
        public ReleaseRights ReleaseDefinitionsRightsProjectAdmin { get; set; }

        public ReleaseRights ReleaseRightsProductionEnvOwner { get; set; }
        public ReleaseRights ReleaseDefintionsRightsProductionEnvOwner { get; set; }
        
        public ReleaseRights ReleaseRightsReleaseAdmin { get; set; }
        public ReleaseRights ReleaseDefintionsRightsReleaseAdmin { get; set; }

        public bool ProjectIsSecure => ApplicationGroupContainsProductionEnvironmentOwner &&
                                       ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup &&
                                       TeamRabobankProjectAdministrators.GlobalRightsIsSecure &&
                                       BuildRightsBuildAdmin.IsSecure &&
                                       BuildRightsProjectAdmin.IsSecure &&
                                       BuildRightsContributor.IsSecure &&
                                       BuildDefinitionsRightsProjectAdmin.IsSecure &&
                                       BuildDefinitionsRightsBuildAdmin.IsSecure &&
                                       BuildDefinitionsRightsContributor.IsSecure &&
                                       RepositoryRightsProjectAdmin.RepositoryRightsIsSecure &&
                                       ReleaseRightsRaboProjectAdmin.IsSecure &&
                                       ReleaseDefinitionsRightsRaboProjectAdmin.IsSecure &&
                                       ReleaseRightsContributor.IsSecure &&
                                       ReleaseDefintionsRightsContributor.IsSecure &&
                                       ReleaseRightsProductionEnvOwner.IsSecure &&
                                       ReleaseDefintionsRightsProductionEnvOwner.IsSecure &&
                                       ReleaseRightsProjectAdmin.IsSecure &&
                                       ReleaseDefinitionsRightsProjectAdmin.IsSecure &&
                                       ReleaseRightsReleaseAdmin.IsSecure &&
                                       ReleaseDefintionsRightsReleaseAdmin.IsSecure
        ;

        public DateTime Date { get; }
    }
}