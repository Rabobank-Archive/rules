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
        readonly IVstsRestClient _client;
        private const string MavenTaskId = "ac4ee482-65da-4485-a532-7b085873e532";

        public GuiPipelineEvaluator(IVstsRestClient client)
        {
            _client = client;
        }

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline, IPipelineHasTaskRule rule)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            if (buildPipeline.Process.Phases == null)
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));

            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            bool? result = await DoesPipelineContainTaskAsync(project, buildPipeline, rule.TaskId).ConfigureAwait(false);

            if (!result.GetValueOrDefault() &&
                await DoesPipelineContainTaskAsync(project, buildPipeline, MavenTaskId).ConfigureAwait(false)
                )
                result = null;

            return result;
        }

        private async Task<bool> DoesPipelineContainTaskAsync(Project project, BuildDefinition buildPipeline, string taskId) =>
            await BuildStepsContainsTaskAsync(
                project,
                buildPipeline.Process.Phases
                    .Where(p => p.Steps != null)
                    .SelectMany(p => p.Steps),
                taskId)
                .ConfigureAwait(false);

        private async Task<bool> BuildStepsContainsTaskAsync(Project project, IEnumerable<BuildStep> steps, string taskId)
        {
            var found = BuildStepsContainTask(steps, taskId);
            var queue = new Queue<string>(GetTaskGroups(steps));
            var done = new HashSet<string>();

            while (!found && queue.Any())
            {
                var todo = new HashSet<Task<IEnumerable<BuildStep>>>();
                while (queue.Any())
                {
                    var taskGroupId = queue.Dequeue();
                    if (done.Contains(taskGroupId))
                        continue;

                    done.Add(taskGroupId);

                    todo.Add(GetTaskGroupStepsAsync(project, taskGroupId));
                }

                while (!found && todo.Any())
                {
                    var completed = await System.Threading.Tasks.Task
                        .WhenAny(todo)
                        .ConfigureAwait(false);

                    if (completed.Status == TaskStatus.RanToCompletion)
                    {
                        found = BuildStepsContainTask(completed.Result, taskId);

                        foreach (var id in GetTaskGroups(completed.Result))
                            queue.Enqueue(id);
                    }

                    todo.Remove(completed);
                }
            }

            return found;
        }

        private async Task<IEnumerable<BuildStep>> GetTaskGroupStepsAsync(Project project, string taskGroup) =>
            (await _client.GetAsync(VstsService.Requests.TaskGroup.TaskGroupById(project.Id, taskGroup))
                          .ConfigureAwait(false)).Value.FirstOrDefault()?.Tasks ?? new BuildStep[0];

        private static IEnumerable<string> GetTaskGroups(IEnumerable<BuildStep> steps) =>
            steps.Where(s => s.Enabled && s.Task.DefinitionType == "metaTask")
                 .Select(s => s.Task.Id);

        private static bool BuildStepsContainTask(IEnumerable<BuildStep> steps, string taskId) => steps.Any(s => s.Enabled && s.Task.Id == taskId);
    }
}