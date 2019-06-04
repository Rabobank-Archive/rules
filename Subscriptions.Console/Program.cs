using Microsoft.Extensions.CommandLineUtils;
using SecurePipelineScan.VstsService;
using System;
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
            var deleteOption = app.Option("-d|--delete", "if this command should delete service hooks instead of creating them add -d true", CommandOptionType.NoValue);

            // Currently only check build.completed
            app.OnExecute(() =>
            {
                if (!tokenOption.HasValue() ||
                    !organizationOption.HasValue() ||
                    !accountNameOption.HasValue())
                {
                    app.ShowHelp();
                    return 1;
                }

                var client = new VstsRestClient(organizationOption.Value(), tokenOption.Value(), new RestClientFactory());
                var subscriptions = client
                    .Get(Requests.Hooks.Subscriptions())
                    .Where(_ => _.ConsumerId == "azureStorageQueue");

                if (deleteOption.Values.Any())
                {
                    RemoveStorageHook(accountNameOption.Value(), client, subscriptions);
                }
                else
                {
                    if (!accountKeyOption.HasValue())
                    {
                        app.ShowHelp();
                        return 1;
                    }
                    var projects = client
                        .Get(Requests.Project.Projects());

                    var items = SubscriptionsPerProject(subscriptions, projects);
                    AddHooksToProjects(accountNameOption.Value(), accountKeyOption.Value(), client, items);
                }
                return 0;
            });

            app.Execute(args);
        }

        private static void RemoveStorageHook(string storageAccountName, VstsRestClient client, IEnumerable<Response.Hook> subscriptions)
        {
            foreach (var subscription in subscriptions)
            {
                if (subscription.ActionDescription.Contains(storageAccountName))
                {
                    client.Delete(Requests.Hooks.Subscription(subscription.Id));
                }

            }
        }

        internal static void AddHooksToProjects(string accountName, string accountKey, IVstsRestClient client, IEnumerable<ProjectInfo> items)
        {
            List<Exception> aggregateExceptions = new List<Exception>();
            foreach (var item in items)
            {
                try
                {
                    AddHooksToProject(accountName, accountKey, client, item);
                }
                catch (Exception e)
                {
                    aggregateExceptions.Add(e);
                }
            }
            if (aggregateExceptions.Count > 0)
            {
                throw new AggregateException(aggregateExceptions);
            }
        }

        private static void AddHooksToProject(string accountName, string accountKey, IVstsRestClient client, ProjectInfo item)
        {
            if (!item.BuildComplete)
            {
                System.Console.WriteLine($"Add build.completed subscription to project: {item.Id}");
                client.Post(Requests.Hooks.AddHookSubscription(), Requests.Hooks.Add.BuildCompleted(accountName, accountKey, "buildcompleted", item.Id));
            }
            if (!item.GitPullRequestCreated)
            {
                System.Console.WriteLine($"Add GitPullRequestCreated subscription to project: {item.Id}");
                client.Post(Requests.Hooks.AddHookSubscription(), Requests.Hooks.Add.GitPullRequestCreated(accountName, accountKey, "pullrequestcreated", item.Id));
            }
            if (!item.GitPushed)
            {
                System.Console.WriteLine($"Add GitPushed subscription to project: {item.Id}");
                client.Post(Requests.Hooks.AddHookSubscription(), Requests.Hooks.Add.GitPushed(accountName, accountKey, "gitpushed", item.Id));
            }
            if (!item.ReleaseDeploymentCompleted)
            {
                System.Console.WriteLine($"Add Release deployment completed subscription to project: {item.Id}");

                // We make sure the Release definition module is loaded.
                client.Get(Requests.ReleaseManagement.Definitions(item.Id));
                client.Post(Requests.Hooks.AddHookSubscription(), Requests.Hooks.Add.ReleaseDeploymentCompleted(accountName, accountKey, "releasedeploymentcompleted", item.Id));
            }
        }

        public static IEnumerable<ProjectInfo> SubscriptionsPerProject(IEnumerable<Response.Hook> subscriptions, IEnumerable<Response.Project> projects)
        {
            var items = projects.Select(x => new ProjectInfo { Id = x.Id }).ToList();
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
                
                default:
                    throw new ArgumentException(eventType, nameof(eventType));
            }
        }
    }
}