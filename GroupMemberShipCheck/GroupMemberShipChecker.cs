using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.GroupMemberShipCheck
{
    internal class GroupMemberShipChecker
    {
        private IVstsRestClient client;

        public CheckResults Execute(IVstsRestClient client, string groupName, List<string> okProjectNames, List<string> notOkProjectNames, NameValueCollection notOkProjectMembers)
        {
            if (notOkProjectNames == null)
            {
                throw new ArgumentNullException(nameof(notOkProjectNames));
            }

            this.client = client;
 
            //string groupName = "Project Administrators";
            //List<string> okProjectNames = new List<string>();
            //List<string> notOkProjectNames = new List<string>();
            //NameValueCollection notOkProjectMembers = new NameValueCollection();

            var projects = client.Get(Requests.Project.Projects()).Value;

            return FetchGroupmembersOfProjects(groupName, projects);
            /*
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
            } */
        }

        private CheckResults FetchGroupmembersOfProjects(string groupName, IEnumerable<VstsService.Response.Project> projects)
        {
            CheckResults checkResults = new CheckResults()
            {
                okProjectNames = new List<string>(),
                notOkProjectNames = new List<string>(),
                notOkProjectMembers = new NameValueCollection()
            };
            foreach (var project in projects)
            {
                var projectId = project.Id;
                var groupMembers = FetchGroupMembers(groupName, projectId);

                var membershipsOk = false;

                foreach (var identity in groupMembers.Identities)
                {
                    // Ok only if there's one group "<projectnaam> \ Rabobank Project Administrators" and no further other members in "Project Administrators".
                    // create 2 NameValue collections, one OK and one notOK  with projectname, members 
                    if (identity.DisplayName.Contains("[" + project.Name + "]\\Rabobank Project Administrators"))
                    {
                        membershipsOk = true;
                    }
                    else
                    {
                        checkResults.notOkProjectMembers.Add(project.Name, identity.DisplayName);
                        membershipsOk = false;
                    }
                }

                if (membershipsOk)
                {
                    checkResults.okProjectNames.Add(project.Name);
                }
                else
                {
                    checkResults.notOkProjectNames.Add(project.Name);
                }
            }
            return checkResults;
        }

        private VstsService.Response.Security.IdentityGroup FetchGroupMembers(string groupName, string projectId)
        {
            try
            {
                var groupId = client.Get(Requests.Security.Groups(projectId)).Identities.Single(x => x.FriendlyDisplayName == groupName).TeamFoundationId;
                return client.Get(Requests.Security.GroupMembers(projectId, groupId));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception raised" + ex + "\n\n");
                throw;
            }
        }
    }
}