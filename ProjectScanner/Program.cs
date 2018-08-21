using Microsoft.Extensions.CommandLineUtils;
using System;
using VstsService;

namespace ProjectScanner
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
                var projectName = projectNameOption.Value();

                string encodedPath = Base64Encode($":{token}");
                var scanner = new VstsScanner($"Basic {encodedPath}", "somecompany");
                var result = scanner.ScanProject(projectName);
                
                Console.Write(result.ToCsv($"\t"));

                return 0;
            });
            app.Execute(args);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}