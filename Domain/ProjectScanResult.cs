using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        /// <summary>
        /// Exports the Results to a CSV-string with a delimiter. Defaults to a semicolon (;)
        /// </summary>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public string ToCsv(string delimiter = ";")
        {
            StringWriter sw = new StringWriter();

            sw.WriteLine($"Project: {Name}");
            sw.WriteLine($"Release Definitions:");

            foreach (var releaseDef in ReleaseDefinitions)
            {
                sw.WriteLine($"{releaseDef.Name}");
                sw.WriteLine($"{delimiter}Env:");

                foreach (var env in releaseDef.Environments)
                {
                    sw.WriteLine($"{delimiter}{delimiter}{env.Name}{delimiter}{env.ArtifactFilter}{string.Join(",", env.PreDeployApprovers)}");
                }
            }
            sw.WriteLine($"Builds:");

            foreach (var build in ReleaseDefinitions.SelectMany(x => x.BuildDefinitions).Distinct())
            {
                sw.WriteLine($"{delimiter}{build.Name}{delimiter}{build.BuildId}");
            }

            sw.WriteLine($"Code Repositories");
            foreach (var repo in Repositories)
            {
                sw.WriteLine($"Name{delimiter}MasterBranchIsReadOnly{delimiter}RequireAMinimumOfReviewers");
                sw.WriteLine($"{repo.Name}{delimiter}{repo.MasterBranchIsReadOnly}{delimiter}{repo.RequireAMinimumOfReviewers}");
            }
            return sw.ToString();
        }
    }
}