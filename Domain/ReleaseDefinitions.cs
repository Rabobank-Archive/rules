using System.Collections.Generic;

namespace Domain
{
    public class ReleaseDefinition
    {
        public string Name { get; set; }

        public IEnumerable<Environment> Environments { get; set; }

        public int Id { get; set; }

        public IEnumerable<BuildDefinition> BuildDefinitions { get; set; }
    }
}