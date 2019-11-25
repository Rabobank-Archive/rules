using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using System;
using SecurePipelineScan.VstsService;
using System.Collections.Generic;

namespace SecurePipelineScan.Rules.Security
{
    public class GuiPipelineEvaluator : IPipelineEvaluator
    {
        private readonly IVstsRestClient _client;
        private const string MavenTaskId = "ac4ee482-65da-4485-a532-7b085873e532";

        public GuiPipelineEvaluator(IVstsRestClient client)
        {
            _client = client;
        }

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline,
            IPipelineHasTaskRule rule)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            if (buildPipeline.Process.Phases == null)
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));

            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            bool? result = await DoesPipelineContainTaskAsync(project, buildPipeline, rule.TaskId)
                .ConfigureAwait(false);

            if (!result.GetValueOrDefault() &&
                await DoesPipelineContainTaskAsync(project, buildPipeline, MavenTaskId).ConfigureAwait(false)
            ) result = null;

            return result;
        }

        private async Task<bool> DoesPipelineContainTaskAsync(Project project, BuildDefinition buildPipeline,
            string taskId) =>
            await BuildStepsContainsTaskAsync(
                    project,
                    buildPipeline.Process.Phases
                        .Where(p => p.Steps != null)
                        .SelectMany(p => p.Steps).ToArray(),
                    taskId)
                .ConfigureAwait(false);

        private async Task<bool> BuildStepsContainsTaskAsync(Project project, IReadOnlyCollection<BuildStep> steps,
            string taskId)
        {
            var found = BuildStepsContainTask(steps, taskId);
            var queue = GetTaskGroups(steps).ToList();
            var done = new HashSet<string>();

            while (!found && queue.Any())
            {
                var todo = queue.Where(q => !done.Contains(q)).ToList();
                var results = await System.Threading.Tasks.Task
                    .WhenAll(todo.Select(q => GetTaskGroupStepsAsync(project, q)))
                    .ConfigureAwait(false);
                var buildSteps = results.SelectMany(r => r).ToArray();
                found = BuildStepsContainTask(buildSteps, taskId);

                done.UnionWith(queue);
                queue = GetTaskGroups(buildSteps).ToList();
            }

            return found;
        }

        private async Task<IEnumerable<BuildStep>> GetTaskGroupStepsAsync(Project project, string taskGroup) =>
            (await _client.GetAsync(VstsService.Requests.TaskGroup.TaskGroupById(project.Id, taskGroup))
                .ConfigureAwait(false)).Value?.FirstOrDefault()?.Tasks ?? new BuildStep[0];

        private static IEnumerable<string> GetTaskGroups(IEnumerable<BuildStep> steps) =>
            steps.Where(s => s.Enabled && s.Task.DefinitionType == "metaTask")
                .Select(s => s.Task.Id);

        private static bool BuildStepsContainTask(IEnumerable<BuildStep> steps, string taskId) =>
            steps.Any(s => s.Enabled && s.Task.Id == taskId);
    }
}