using System.Collections;
using System.Collections.Generic;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using System.Linq;

namespace SecurePipelineScan.Rules
{
    public class SecurityReportScan
    {
        private readonly IVstsRestClient client;

        public SecurityReportScan(IVstsRestClient client)
        {
            this.client = client;
        }

        public void Execute()
        {
            var projects = client.Get(Project.Projects()).Value;
            foreach (var project in projects)
            {
                Execute(project.Name);
            }
        }


        public SecurityReport Execute(string project)
        {
            var applicationGroups =
                client.Get(ApplicationGroup.ApplicationGroups(project)).Identities;
            
            

            var nameSpaces = client.Get(SecurityNamespace.SecurityNamespaces()).Value;

            var queryNameSpaceId =
                from ns in nameSpaces
                where ns.DisplayName == "Git Repositories"
                select ns.NamespaceId;

            var groupIdentities = client.Get(ApplicationGroup.ApplicationGroups(project)).Identities;

            var queryApplicationGroupId =
                from gi in groupIdentities
                where gi.DisplayName == $"[{project}]\\Project Administrators"
                select gi.TeamFoundationId;

            var projectId = client.Get(Project.Properties(project)).Id;

            var permissionsGitRepositorySet = client.Get(PermissionsGroupRepoSet.PermissionsGitRepositorySet(
                projectId, queryNameSpaceId.First(), queryApplicationGroupId.First()));

            var applicationGroupList = applicationGroups.ToList();
            var securityReport = new SecurityReport
            {
                Project = project,
                ApplicationGroupContainsProductionEnvironmentOwner =
                    ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(applicationGroupList),

                ProjectAdminHasNoPermissionsToDeleteRepositorySet =
                    Permission.HasNoPermissionToDeleteRepository(permissionsGitRepositorySet.Permissions),

                ProjectAdminHasNoPermissionToManagePermissionsRepositorySet =
                    Permission.HasNoPermissionToManageRepositoryPermissions(permissionsGitRepositorySet.Permissions),

                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup = CheckRabobankProjectAdminInProjectAdminGroup(project, applicationGroupList),
                    
            };

            return securityReport;
        }

        private bool CheckRabobankProjectAdminInProjectAdminGroup(string project, IEnumerable<VstsService.Response.ApplicationGroup> applicationGroups)
        {
            bool projectAdminHasOnlyRaboProjectAdminGroup = false;
            var groupid = applicationGroups.Single(x => x.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            var groupmembers = client.Get(ApplicationGroup.GroupMembers(project, groupid)).Identities;
            var count = groupmembers.Count();


            if (count == 1)
            {
                var applicationGroup = groupmembers.First(x => x.FriendlyDisplayName == "Rabobank Project Administrators");
                projectAdminHasOnlyRaboProjectAdminGroup = applicationGroup != null;
            }

            return projectAdminHasOnlyRaboProjectAdminGroup;
        }
    }
}