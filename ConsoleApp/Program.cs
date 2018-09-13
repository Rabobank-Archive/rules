using Microsoft.Extensions.CommandLineUtils;
using SecurePipelineScan.Rules;
using SecurePipelineScan.VstsService;
using System;
using System.Threading.Tasks;

namespace SecurePipelineScan.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
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

                if (String.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("Please add your PAT using -t");
                }

                await Task.Run(() => {
                    var client = new VstsRestClient(organization, token);
                    var scan = new Scan(client, Console.WriteLine);
                    scan.Execute(projectNameOption.Value());
                });
                return 0;
            });
            app.Execute(args);
        }
    }
}