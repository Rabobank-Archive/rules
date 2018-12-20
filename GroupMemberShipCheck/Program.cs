using Microsoft.Extensions.CommandLineUtils;
using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("GroupMemberShipCheck.Tests")]

namespace SecurePipelineScan.GroupMemberShipCheck
{
    internal static class Program
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
                }

                await Task.Run(() =>
                {
                    var checker = new GroupMemberShipChecker();
                   
                    var client = new VstsRestClient(organization, token);
                    string groupName = "Project Administrators";
                    List<string> okProjectNames = new List<string>();
                    List<string> notOkProjectNames = new List<string>();
                    NameValueCollection notOkProjectMembers = new NameValueCollection();

                    CheckResults result = checker.Execute(client, groupName, okProjectNames, notOkProjectNames, notOkProjectMembers);

                    okProjectNames.Sort();
                    NameValueCollection notOkProjectMembersSorted = new NameValueCollection();
                    String[] sortedKeys = notOkProjectMembers.AllKeys;
                    Array.Sort(sortedKeys);
                    foreach (String sortedKey in sortedKeys)
                    {
                        notOkProjectMembersSorted.Add(sortedKey, notOkProjectMembers[sortedKey]);
                    }

                    Console.WriteLine("Compliant projects: (" + okProjectNames.Count + "):");
                    foreach (var okProjectName in okProjectNames)
                    {
                        Console.WriteLine(okProjectName);
                    }

                    Console.WriteLine("Non-Compliant projects: (" + notOkProjectMembersSorted.Count + "):");
                    foreach (var notOkProjectName in notOkProjectNames)
                    {
                        Console.WriteLine("Project non compliant: " + notOkProjectName);
                        Console.WriteLine("\tMembers who should not be here:");
                        foreach (string projectName in notOkProjectMembersSorted.AllKeys.Where(x => x == notOkProjectName))
                        {
                            foreach (string value in notOkProjectMembersSorted.GetValues(projectName))
                            {
                                if (projectName == notOkProjectName)
                                {
                                    Console.WriteLine("\t" + value);
                                }
                            }
                        }
                    }
                });

                return 0;
            });
            app.Execute(args);
        }
    }
}