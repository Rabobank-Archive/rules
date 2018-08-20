using System.Collections.Generic;

namespace Domain
{
    public class ReleaseDefinition
    {
        public string Name { get; set; }
        public IEnumerable<Environment> Environments { get; set; }
        public int Id { get; set; }
        public IEnumerable<BuilDefinition> BuildDefinitions { get; set; }
    }

    public class BuilDefinition
    {
        public string ProjectId { get; set; }

        public string BuildId { get; set; }
        public string Name { get; set; }
    }
}