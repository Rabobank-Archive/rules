using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class ReleaseDefinitionEnvironment
    {
        public string Name { get; set; }
        public List<DeployPhase> DeployPhases { get; set; }
        public RetentionPolicy RetentionPolicy { get; set; }
        public PreDeployApprovals PreDeployApprovals { get; set; }
    }
}