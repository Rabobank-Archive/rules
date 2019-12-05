using SecurePipelineScan.VstsService.Response;
using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleasePipelineHasSm9ChangeTask : IReleasePipelineRule
    {
        private readonly GuiPipelineEvaluator _pipelineEvaluator;
        private const string TaskId = "d0c045b6-d01d-4d69-882a-c21b18a35472";

        public ReleasePipelineHasSm9ChangeTask(IVstsRestClient client) => 
            _pipelineEvaluator = new GuiPipelineEvaluator(client);

        string IRule.Description => "Release pipeline contains SM9 Create Change task";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/NRV1D";
        bool IRule.IsSox => false;

        public async Task<bool?> EvaluateAsync(
            string projectId, string stageId, ReleaseDefinition releasePipeline)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));
            if (releasePipeline.Environments == null)
                throw new ArgumentOutOfRangeException(nameof(releasePipeline));

            var tasks = releasePipeline.Environments
                .SelectMany(e => e.DeployPhases)
                .SelectMany(d => d.WorkflowTasks)
                .ToList();

            var found = PipelineContainsTask(tasks);
            var queue = GetTaskGroups(tasks).ToList();

            return await _pipelineEvaluator.EvaluateTaskGroupsAsync(found, queue, projectId, TaskId)
                .ConfigureAwait(false);
        }

        private static bool PipelineContainsTask(IEnumerable<WorkflowTask> tasks) =>
            tasks.Any(t => t.Enabled && t.TaskId.ToString() == TaskId);

        private static IEnumerable<string> GetTaskGroups(IEnumerable<WorkflowTask> tasks) =>
            tasks.Where(t => t.Enabled && t.DefinitionType == "metaTask")
                .Select(t => t.TaskId.ToString());
    }
}