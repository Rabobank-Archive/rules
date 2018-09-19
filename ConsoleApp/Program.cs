using SecurePipelineScan.Rules;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.Rules.Reports;
using Microsoft.Extensions.CommandLineUtils;
using System;
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
                    var scan = new Scan(client, Print);
                    scan.Execute(projectNameOption.Value());
                });
                return 0;
            });
            app.Execute(args);
        }

        private static void Print(ScanReport progress)
        {
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

            switch (progress)
            {
                case ReleaseReport r:
                    Console.WriteLine($"    {progress.Request.Id} {progress.Request.Owner.Name}: {ColorCode(r.Result)}");
                    break;

                default:
                    Console.WriteLine($"    {progress.Request.Definition.Name} {progress.Request.Owner.Name}: \u001b[93m?\u001b[0m");
                    break;
            }
        }
        private static string ColorCode(bool result)
        {
            return result ? "\u001b[32mV\u001b[0m" : "\u001b[31mX\u001b[0m";
        }
    }
}