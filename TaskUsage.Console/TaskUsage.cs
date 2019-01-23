using System;

namespace TaskUsage
{
    public class TaskUsage
    {
        public Guid TaskId { get; set; }

        public string ReleaseDefinition { get; set; }

        public string ReleaseDefinitionName { get; set; }

        public string ProjectName { get; set; }

        public string EnvironmentName { get; set; }

        public string TaskVersion { get; set; }

        public string Name { get; internal set; }

        public string Link { get; internal set; }
    }
}