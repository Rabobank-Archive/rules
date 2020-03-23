using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;
using TaskGroup = SecurePipelineScan.VstsService.Requests.TaskGroup;

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

        public async Task<bool?> EvaluateAsync(
            Project project, BuildDefinition buildPipeline, IPipelineHasTaskRule rule)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (buildPipeline == null)
            {
                throw new ArgumentNullException(nameof(buildPipeline));
            }

            if (buildPipeline.Process.Phases == null)
            {
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            bool? result = await BuildPipelineContainsTaskAsync(project.Id, buildPipeline, rule)
                .ConfigureAwait(false);

            if (!result.GetValueOrDefault() &&
                await BuildPipelineContainsTaskAsync(project.Id, buildPipeline, new PipelineHasTaskRule(MavenTaskId))
                    .ConfigureAwait(false))
            {
                result = null;
            }

            return result;
        }

        private async Task<bool> BuildPipelineContainsTaskAsync(
            string projectId, BuildDefinition buildPipeline, IPipelineHasTaskRule rule)
        {
            var steps = buildPipeline.Process.Phases
                .Where(p => p.Steps != null)
                .SelectMany(p => p.Steps)
                .ToList();

            var found = BuildStepsContainTask(steps, rule);
            var queue = GetTaskGroups(steps).ToList();

            return await EvaluateTaskGroupsAsync(found, queue, projectId, rule)
                .ConfigureAwait(false);
        }

        private async Task<IEnumerable<BuildStep>> GetTaskGroupStepsAsync(string projectId, string taskGroup)
        {
            return (await _client.GetAsync(TaskGroup.TaskGroupById(projectId, taskGroup))
                       .ConfigureAwait(false)).Value?.FirstOrDefault()?.Tasks ?? new BuildStep[0];
        }

        private static IEnumerable<string> GetTaskGroups(IEnumerable<BuildStep> steps)
        {
            return steps.Where(s => s.Enabled && s.Task.DefinitionType == "metaTask")
                .Select(s => s.Task.Id);
        }

        private static bool BuildStepsContainTask(IEnumerable<BuildStep> steps, IPipelineHasTaskRule rule)
        {
            var step = steps.FirstOrDefault(s => s.Enabled && s.Task.Id == rule.TaskId);
            if (step == null)
                return false;

            var ruleInputs = rule.Inputs;
            if (ruleInputs == null || !ruleInputs.Any())
                return true;

            return BuildStepContainsRequiredInput(step, ruleInputs);
        }

        private static bool BuildStepContainsRequiredInput(BuildStep step, Dictionary<string, string> ruleInputs)
        {
            if (step.Inputs == null)
                return false;

            foreach (var ruleInput in ruleInputs)
            {
                if (!step.Inputs.ContainsKey(ruleInput.Key))
                    return false;

                var stepInput = step.Inputs[ruleInput.Key];
                if ((stepInput == null && ruleInput.Value == null) || 
                        (stepInput != null && stepInput.Equals(ruleInput.Value, StringComparison.InvariantCultureIgnoreCase)))
                    return true;
            }
            return false;
        }

        public async Task<bool> EvaluateTaskGroupsAsync(
            bool found, IList<string> queue, string projectId, IPipelineHasTaskRule rule)
        {
            var done = new HashSet<string>();

            while (!found && queue.Any())
            {
                var todo = queue.Where(q => !done.Contains(q)).ToList();
                var buildSteps = (await Task.WhenAll(todo.Select(q => GetTaskGroupStepsAsync(projectId, q)))
                    .ConfigureAwait(false)).SelectMany(s => s).ToList();
                found = BuildStepsContainTask(buildSteps, rule);
                done.UnionWith(queue);
                queue = GetTaskGroups(buildSteps).ToList();
            }

            return found;
        }
    }
}