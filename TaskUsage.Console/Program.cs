using Microsoft.Extensions.CommandLineUtils;
using SecurePipelineScan.VstsService;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TaskUsage.Console
{
    using Console = System.Console;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            var tokenOption = app.Option("-t|--token <token>", "The personal access token",
                CommandOptionType.SingleValue);
            var organizationOption = app.Option("-o|--organization <organization>", "The vsts organization", CommandOptionType.SingleValue);

            app.OnExecute(async () =>
            {
                var token = tokenOption.Value();
                var organization = organizationOption.Value();

                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("Please add your PAT using -t");
                    return 0;
                }

                if (string.IsNullOrWhiteSpace(organization))
                {
                    Console.WriteLine("Please add your organization using -o");
                    return 0;
                }

                await Task.Run(() =>
                {
                    var client = new VstsRestClient(organization, token);
                    var taskScanner = new TaskScanner(client);

                    var contents = taskScanner.CreateOverview();

                    File.WriteAllText($"taskoverview_{DateTime.UtcNow.Ticks}.csv", contents);
                });
                return 0;
            });
            app.Execute(args);

            Console.ReadLine();
        }
    }
}