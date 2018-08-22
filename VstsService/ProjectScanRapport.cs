using System.Collections.Generic;
using System.Linq;

namespace VstsService
{
    public class ProjectScanRapport
    {
        public ProjectScanRapport(string projectName)
        {
            Releases = Enumerable.Empty<Domain.Release>();
            this.ProjectName = projectName;
        }

        public string ProjectName { get; }

        public IEnumerable<Domain.Release> Releases { get; set; }
    }
}