using System.Collections.Generic;

namespace Domain
{
    public class Environment
    {
        public string Name { get; set; }

        public bool ArtifactFilter { get; set; }
        public IEnumerable<string> PreDeployApprovers { get; set; }
    }
}