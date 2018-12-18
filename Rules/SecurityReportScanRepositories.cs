using System.Collections;
using System.Collections.Generic;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using System.Linq;

namespace SecurePipelineScan.Rules
{
    public class SecurityReportScanRepositories
    {
        private readonly IVstsRestClient client;

        public SecurityReportScanRepositories(IVstsRestClient client)
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
            var nameSpaces = client.Get(NameSpace.NameSpaces()).Value;

            var queryNameSpaceId =
                from ids in nameSpaces
                where ids.DisplayName == "Git Repositories"
                select ids.namespaceId;
            
            var groupIdentities = client.Get(ApplicationGroup.ApplicationGroups(project)).Identities;
            
            var queryApplicationGroupId = 
                from ids in groupIdentities
                where ids.DisplayName == $"[{project}]\\Project Administrators"
                select ids.TeamFoundationId;
         
            var projectId = client.Get(Project.ProjectProperties(project)).Id;
            
            
            var permissionsGitRepositorySet = client.Get(PermissionsGroupRepoSet.PermissionsGitRepositorySet(
                projectId, queryNameSpaceId.First(), queryApplicationGroupId.First()));
            
            var securityReport = new SecurityReport
            {
                ProjectAdminHasNoPermissionsToDeleteRepositorySet = 
                    Permission.HasNoPermissionToDeleteRepository(permissionsGitRepositorySet.Permissions),
                ProjectAdminHasNoPermissionToManagePermissionsRepositorySet = 
                    Permission.HasNoPermissionToManageRepositoryPermissions(permissionsGitRepositorySet.Permissions)
            };

            return securityReport;
        }

    }
}