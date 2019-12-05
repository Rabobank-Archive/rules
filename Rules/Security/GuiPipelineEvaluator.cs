using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using System;
using SecurePipelineScan.VstsService;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class GuiPipelineEvaluator : IPipelineEvaluator
    {
        private readonly IVstsRestClient _client;
        private const string MavenTaskId = "ac4ee482-65da-4485-a532-7b085873e532";

        public GuiPipelineEvaluator(IVstsRestClient client) => _client = client;

        public async Task<bool?> EvaluateAsync(
            Project project, BuildDefinition buildPipeline, IPipelineHasTaskRule rule)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));
            if (buildPipeline.Process.Phases == null)
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            bool? result = await BuildPipelineContainsTaskAsync(project.Id, buildPipeline, rule.TaskId)
                .ConfigureAwait(false);

            if (!result.GetValueOrDefault() && 
                await BuildPipelineContainsTaskAsync(project.Id, buildPipeline, MavenTaskId)
                    .ConfigureAwait(false))
                result = null;

            return result;
        }

        private async Task<bool> BuildPipelineContainsTaskAsync(
            string projectId, BuildDefinition buildPipeline, string taskId)
        {
            var steps = buildPipeline.Process.Phases
                .Where(p => p.Steps != null)
                .SelectMany(p => p.Steps)
                .ToList();

            var found = BuildStepsContainTask(steps, taskId);
            var queue = GetTaskGroups(steps).ToList();

            return await EvaluateTaskGroupsAsync(found, queue, projectId, taskId)
                .ConfigureAwait(false);
        }

        private async Task<IEnumerable<BuildStep>> GetTaskGroupStepsAsync(string projectId, string taskGroup) =>
            (await _client.GetAsync(VstsService.Requests.TaskGroup.TaskGroupById(projectId, taskGroup))
                .ConfigureAwait(false)).Value?.FirstOrDefault()?.Tasks ?? new BuildStep[0];

        private static IEnumerable<string> GetTaskGroups(IEnumerable<BuildStep> steps) =>
            steps.Where(s => s.Enabled && s.Task.DefinitionType == "metaTask")
                .Select(s => s.Task.Id);

        private static bool BuildStepsContainTask(IEnumerable<BuildStep> steps, string taskId) =>
            steps.Any(s => s.Enabled && s.Task.Id == taskId);

        public async Task<bool> EvaluateTaskGroupsAsync(
            bool found, IList<string> queue, string projectId, string taskId)
        {
            var done = new HashSet<string>();

            while (!found && queue.Any())
            {
                var todo = queue.Where(q => !done.Contains(q)).ToList();
                var buildSteps = (await Task.WhenAll(todo.Select(q => GetTaskGroupStepsAsync(projectId, q)))
                    .ConfigureAwait(false)).SelectMany(s => s).ToList();
                found = BuildStepsContainTask(buildSteps, taskId);
                done.UnionWith(queue);
                queue = GetTaskGroups(buildSteps).ToList();
            }

            return found;
        }
    }
}