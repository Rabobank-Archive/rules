using System.Collections.Generic;

namespace Domain
{
    public class ProjectScanResult
    {
        public string Name { get; set; }

        public ProjectScanResult(string name)
        {
            Name = name;
        }

        /// <summary>
        /// All Release definitions in a Project
        /// </summary>
        public IEnumerable<ReleaseDefinition> ReleaseDefinitions { get; set; } = new List<ReleaseDefinition>();

        /// <summary>
        /// All Build Definitions in a Project
        /// </summary>
        public IEnumerable<BuildDefinition> BuildDefinitions { get; set; } = new List<BuildDefinition>();

        /// <summary>
        /// All Source Control Repositories in a Project
        /// </summary>
        public IEnumerable<Repository> Repositories { get; set; } = new List<Repository>();
    }
}