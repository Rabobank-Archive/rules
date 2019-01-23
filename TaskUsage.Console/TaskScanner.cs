using SecurePipelineScan.VstsService;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskUsage.Console
{
    public class TaskScanner
    {
        private readonly IVstsRestClient client;
        private readonly StringBuilder stringBuilder;

        public TaskScanner(IVstsRestClient client)
        {
            this.client = client;
        }

        public string CreateOverview()
        {
            var allTasks = client.Get(SecurePipelineScan.VstsService.Requests.DistributedTask.Tasks());

            var allDefinitions = GetDefinitions(client);

            var taskUsages = GetTaskUsage(allDefinitions);

            var overview = taskUsages.GroupBy(
                p => p.TaskId,
                p => p,
                (taskId, g) =>
                new TaskOverview
                {
                    TaskId = taskId,
                    Count = g.Count(),
                    TaskUsage = g,
                    TaskName = allTasks.FirstOrDefault(x => x.Id == taskId.ToString())?.Name,
                });

            StringBuilder sb = new StringBuilder();
            sb.Append(overview.ToCsv());
            sb.AppendLine();
            sb.Append(taskUsages.ToCsv());

            return sb.ToString();
        }

        private static IEnumerable<SecurePipelineScan.VstsService.Response.ReleaseDefinition> GetDefinitions(IVstsRestClient client)
        {
            foreach (var project in client.Get(SecurePipelineScan.VstsService.Requests.Project.Projects()))
            {
                foreach (var releaseDef in client.Get(SecurePipelineScan.VstsService.Requests.Release.Definitions(project.Name)))
                {
                    yield return client.Get(SecurePipelineScan.VstsService.Requests.Release.Definition(project.Id, releaseDef.Id));
                }
            }
        }

        private static IEnumerable<TaskUsage> GetTaskUsage(IEnumerable<SecurePipelineScan.VstsService.Response.ReleaseDefinition> allDefinitions)
        {
            foreach (var releaseDefinitionEnvironments in allDefinitions)
            {
                foreach (var env in releaseDefinitionEnvironments.Environments)
                {
                    foreach (var deployPhases in env.DeployPhases)
                    {
                        foreach (var workflowTask in deployPhases.WorkflowTasks)
                        {
                            yield return new TaskUsage
                            {
                                TaskId = workflowTask.TaskId,
                                Name = workflowTask.Name,
                                TaskVersion = workflowTask.Version,
                                ReleaseDefinitionName = releaseDefinitionEnvironments.Name,
                                ReleaseDefinition = releaseDefinitionEnvironments.Id,
                                EnvironmentName = env.Name,
                                Link = releaseDefinitionEnvironments.Links.Self.Href.AbsoluteUri
                            };
                        }
                    }
                }
            }
        }
    }
}