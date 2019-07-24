using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class ReleaseDefinitionEnvironment
    {
        public string Name { get; set; }
        public IList<DeployPhase> DeployPhases { get; set; }
        public RetentionPolicy RetentionPolicy { get; set; }
        public PreDeployApprovals PreDeployApprovals { get; set; }
    }
}