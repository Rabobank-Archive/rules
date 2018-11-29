using Microsoft.Extensions.CommandLineUtils;
using SecurePipelineScan.VstsService;
using System.Collections.Generic;
using System.Linq;
using Requests = SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Subscriptions.Console.Tests")]

namespace Subscriptions.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            var tokenOption = app.Option("-t|--token <token>", "The personal access token", CommandOptionType.SingleValue);
            var organizationOption = app.Option("-o|--organization <organization>", "The vsts organization", CommandOptionType.SingleValue);
            var accountNameOption = app.Option("-an|--accountname <accountname>", "The name of the account", CommandOptionType.SingleValue);
            var accountKeyOption = app.Option("-ak|--accountkey <accountkey>", "The key of the account 64 chars", CommandOptionType.SingleValue);

            // Currently only check build.completed
            app.OnExecute(() =>
            {
                if (!tokenOption.HasValue() ||
                    !organizationOption.HasValue() ||
                    !accountNameOption.HasValue() ||
                    !accountKeyOption.HasValue())
                {
                    app.ShowHelp();
                    return 1;
                }

                var client = new VstsRestClient(organizationOption.Value(), tokenOption.Value());
                var subscriptions = client
                    .Execute(Requests.Hooks.Subscriptions())
                    .ThrowOnError().Data.Value
                    .Where(_ => _.ConsumerId == "azureStorageQueue");

                var projects = client
                    .Execute(Requests.Project.Projects())
                    .ThrowOnError().Data.Value;
                
                var items = SubscriptionsPerProject(subscriptions, projects);
                AddHooksToProjects(accountNameOption.Value(), accountKeyOption.Value(), client, items);

                return 0;
            });

            app.Execute(args);
        }

        internal static void AddHooksToProjects(string accountName, string accountKey, IVstsRestClient client, IEnumerable<ProjectInfo> items)
        {
            foreach (var item in items)
            {
                if (!item.BuildComplete)
                {
                    System.Console.WriteLine($"Add build.completed subscription to project: {item.Id}");
                    client.Execute(Requests.Hooks.Add.BuildCompleted(accountName, accountKey, "buildcompleted", item.Id));
                }
                if (!item.GitPullRequestCreated)
                {
                    System.Console.WriteLine($"Add GitPullRequestCreated subscription to project: {item.Id}");
                    client.Execute(Requests.Hooks.Add.GitPullRequestCreated(accountName, accountKey, "pullrequestcreated", item.Id));
                }
                if (!item.GitPushed)
                {
                    System.Console.WriteLine($"Add GitPushed subscription to project: {item.Id}");
                    client.Execute(Requests.Hooks.Add.GitPushed(accountName, accountKey, "gitpushed", item.Id));
                }
                if (!item.ReleaseDeploymentCompleted)
                {
                    System.Console.WriteLine($"Add Release deployment completed subscription to project: {item.Id}");
                    client.Execute(Requests.Hooks.Add.ReleaseDeploymentCompleted(accountName, accountKey, "releasedeploymentcompleted", item.Id));
                }
            }
        }

        public static IEnumerable<ProjectInfo> SubscriptionsPerProject(IEnumerable<Response.Hook> subscriptions, IEnumerable<Response.Project> projects)
        {
            var items = projects.Select(x => new ProjectInfo {Id = x.Id}).ToList();            
            foreach (var subscription in subscriptions)
            {
                var item = items.SingleOrDefault(x => x.Id == subscription.PublisherInputs.ProjectId);
                if (item != null)
                {                    
                    UpdateProjectInfo(item, subscription.EventType);
                }
            }

            return items;
        }

        private static void UpdateProjectInfo(ProjectInfo projectInfo, string eventType)
        {
            switch (eventType)
            {
                case "build.complete":
                    projectInfo.BuildComplete = true;
                    break;

                case "git.pullrequest.created":
                    projectInfo.GitPullRequestCreated = true;
                    break;

                case "git.push":
                    projectInfo.GitPushed = true;
                    break;

                case "ms.vss-release.deployment-completed-event":
                    projectInfo.ReleaseDeploymentCompleted = true;
                    break;
            }
        }
    }
}