using System.Collections.Generic;

namespace VstsService
{
    public class ProjectScanRapport
    {
        public ProjectScanRapport(string projectName)
        {
            ReleaseDefinitions = new List<Domain.ReleaseDefinition>();
            this.ProjectName = projectName;
        }

        public string ProjectName { get; }

        public IEnumerable<Domain.ReleaseDefinition> ReleaseDefinitions { get; set; }
    }
}