using System;
using System.Collections.Generic;

namespace TaskUsage
{
    public class TaskOverview
    {
        public Guid TaskId { get; set; }

        public string TaskName { get; set; }

        public int Count { get; set; }

        public IEnumerable<TaskUsage> TaskUsage { get; set; }
    }
}