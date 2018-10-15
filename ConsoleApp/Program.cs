using Microsoft.Extensions.CommandLineUtils;
using Rules.Reports;
using SecurePipelineScan.Rules;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurePipelineScan.ConsoleApp
{
    internal static class Program
    {
        private static string endpoint;
        private static string definition;

        private static void Main(string[] args)
        {
            ColorsOnWindows.Enable();

            var app = new CommandLineApplication();
            var tokenOption = app.Option("-t|--token <token>", "The personal access token",
                CommandOptionType.SingleValue);
            var organizationOption = app.Option("-o|--organization <organization>", "The vsts organization", CommandOptionType.SingleValue);
            var projectNameOption = app.Option("-p|--project <projectName>", "The project name",
                CommandOptionType.SingleValue);

            app.OnExecute(async () =>
            {
                var token = tokenOption.Value();
                var organization = organizationOption.Value();

                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("Please add your PAT using -t");
                }

                await Task.Run(() =>
                {
                    var client = new VstsRestClient(organization, token);
                    var endPointScan = new EndPointScan(client, Print);
                    endPointScan.Execute(projectNameOption.Value());

                    var repoScan = new RepositoryScan(client, PrintMultiple);
                    repoScan.Execute(projectNameOption.Value());
                });
                return 0;
            });
            app.Execute(args);

            Console.ReadLine();
        }

        private static void Print(ScanReport progress)
        {
            switch (progress)
            {
                case ReleaseReport r:
                    if (endpoint != progress.Endpoint.Id)
                    {
                        Console.WriteLine(progress.Endpoint.Name);
                        endpoint = progress.Endpoint.Id;
                    }

                    if (definition != progress.Request.Definition.Id)
                    {
                        Console.WriteLine($"  {progress.Request.Definition.Name}");
                        definition = progress.Request.Definition.Id;
                    }

                    Console.WriteLine($"    {progress.Request.Id} {progress.Request.Owner.Name}: {ColorCode(r.Result)}");
                    break;

                case RepositoryReport r:
                    //Console.WriteLine($"project - repository - HasRequiredReviewerPolicy");
                    Console.WriteLine($" {r.Project} - {r.Repository} - {ColorCode(r.HasRequiredReviewerPolicy)}");
                    break;

                default:
                    Console.WriteLine($"    {progress.Request.Definition.Name} {progress.Request.Owner.Name}: \u001b[93m?\u001b[0m");
                    break;
            }
        }

        private static void PrintMultiple(IEnumerable<ScanReport> progress)
        {
            foreach (var item in progress)
            {
                Print(item);
            }
        }

        private static string ColorCode(bool result)
        {
            return result ? "\u001b[32mV\u001b[0m" : "\u001b[31mX\u001b[0m";
        }

        private static string ColorCode(bool? result)
        {
            return result.HasValue ? ColorCode(result.Value) : "\u001b[31m-\u001b[0m";
        }
    }
}