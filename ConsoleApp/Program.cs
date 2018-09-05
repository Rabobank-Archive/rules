using Microsoft.Extensions.CommandLineUtils;
using System;

namespace SecurePipelineScan.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            var tokenOption = app.Option("-t|--token <token>", "The personal access token",
                CommandOptionType.SingleValue);
            var urlOption = app.Option("-u|--url <url>", "The vsts/tfs url", CommandOptionType.SingleValue);
            var projectNameOption = app.Option("-p|--project <projectName>", "The project name for the release def",
                CommandOptionType.SingleValue);

            app.OnExecute(async () =>
            {
                var token = tokenOption.Value();
                var url = urlOption.Value();

                if (String.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("Please add your PAT using -t");
                }

                await System.Threading.Tasks.Task.CompletedTask;
                return 0;
            });
            app.Execute(args);
        }
    }
}