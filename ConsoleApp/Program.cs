using Microsoft.Extensions.CommandLineUtils;
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

                    var repoScan = new RepositoryScan(client);
                    PrintMultiple(repoScan.Execute(projectNameOption.Value(), DateTime.Now));
                });
                return 0;
            });
            app.Execute(args);

            Console.ReadLine();
        }

        private static void PrintMultiple(IEnumerable<RepositoryReport> items)
        {
            foreach (var item in items)
            {
                Console.WriteLine($" {item.Project} - {item.Repository} - {ColorCode(item.HasRequiredReviewerPolicy)}");
            }
        }

        private static string ColorCode(bool result)
        {
            return result ? "\u001b[32mV\u001b[0m" : "\u001b[31mX\u001b[0m";
        }
    }
}