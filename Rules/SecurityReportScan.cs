using System;
using System.Collections;
using System.Collections.Generic;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using System.Linq;
using Repository = SecurePipelineScan.VstsService.Response.Repository;

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
            
            var groupMembers = getGroupMembersFromApplicationGroup(project, applicationGroups);

            var namespaceIdGitRepositories = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.DisplayName == "Git Repositories").NamespaceId;

            var namespaceIdBuild = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.Name == "Build").NamespaceId;
            
            var applicationGroupIdProjectAdmins = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Project Administrators").TeamFoundationId;
         
            var applicationGroupIdBuildAdmins = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Build Administrators").TeamFoundationId;
            
            
            var projectId = client.Get(Project.Properties(project)).Id;           
            
            
            var permissionsGitRepositorySet = client.Get(Permissions.PermissionsGroupRepositorySet(
                projectId, namespaceIdGitRepositories, applicationGroupIdProjectAdmins));

            var permissionsBuildProjectAdmins = client.Get(Permissions.PermissionsGroupSetId(
                projectId, namespaceIdBuild, applicationGroupIdProjectAdmins));
            
            var permissionsBuildBuildAdmins = client.Get(Permissions.PermissionsGroupSetId(
                projectId, namespaceIdBuild, applicationGroupIdBuildAdmins));

            
            var repositories = client.Get(VstsService.Requests.Repository.Repositories(projectId)).Value;

            
            var securityReport = new SecurityReport
            {
                Project = project,
                
                ApplicationGroupContainsProductionEnvironmentOwner =
                    ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(applicationGroups),
                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup = 
                    ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(groupMembers),

                RepositoryRightsProjectAdmin = checkRepositoryRights(permissionsGitRepositorySet.Permissions, 
                            repositories, projectId, namespaceIdGitRepositories, applicationGroupIdProjectAdmins),
                
                BuildRightsBuildAdmin = checkBuildRights(permissionsBuildBuildAdmins.Permissions),
                BuildRightsProjectAdmin = checkBuildRights(permissionsBuildProjectAdmins.Permissions), 
            };
          
           return securityReport;
        }

        private RepositoryRights checkRepositoryRights(
            IEnumerable<SecurePipelineScan.VstsService.Response.Permission> permissions, 
            IEnumerable<Repository> repositories, string projectId, string namespaceId, string applicationGroupId)
        {
            return new RepositoryRights
            {
                HasNoPermissionToDeleteRepositories = 
                    ProjectAdminHasNoPermissionToDeleteRepositories(repositories, projectId, namespaceId, applicationGroupId),
                HasNoPermissionToDeleteRepositorySet = 
                    Permission.HasNoPermissionToDeleteRepository(permissions),
                HasNoPermissionToManagePermissionsRepositories = 
                    ProjectAdminHasNoPermissionToManagePermissionsRepositories(repositories, projectId, namespaceId, applicationGroupId),
                HasNoPermissionToManagePermissionsRepositorySet = 
                    Permission.HasNoPermissionToManageRepositoryPermissions(permissions)
            };
        }
        
        private BuildRights checkBuildRights(
            IEnumerable<SecurePipelineScan.VstsService.Response.Permission> permissions)
        {
            return new BuildRights
            {
                HasNoPermissionsToAdministerBuildPermissions = 
                    Permission.HasNoPermissionToAdministerBuildPermissions(permissions),
                HasNoPermissionsToDeleteBuilds = 
                    Permission.HasNoPermissionToDeleteBuilds(permissions),
                HasNoPermissionsToDeDestroyBuilds = 
                    Permission.HasNoPermissionToDestroyBuilds(permissions),
                HasNoPermissionsToDeleteBuildDefinition = 
                    Permission.HasNoPermissionToDeleteBuildDefinition(permissions)
            };
        }
        
        private IEnumerable<VstsService.Response.ApplicationGroup> getGroupMembersFromApplicationGroup(string project, IEnumerable<VstsService.Response.ApplicationGroup> applicationGroups)
        {
            var groupId = applicationGroups.Single(x => x.DisplayName == $"[{project}]\\Project Administrators").TeamFoundationId;
            var groupMembers = client.Get(ApplicationGroup.GroupMembers(project, groupId)).Identities;
            return groupMembers;
        }

        private bool ProjectAdminHasNoPermissionToManagePermissionsRepositories(IEnumerable<Repository> repositories, string projectId, string namespaceId, string applicationGroupId)
        {
            return repositories.All
            (r => 
                Permission.HasNoPermissionToManageRepositoryPermissions(
                    client.Get(Permissions.PermissionsGroupRepository(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions)
                == true);
        }

        private bool ProjectAdminHasNoPermissionToDeleteRepositories(IEnumerable<Repository> repositories, string projectId, string namespaceId, string applicationGroupId)
        {
            return repositories.All
            (r => 
                Permission.HasNoPermissionToDeleteRepository(
                    client.Get(Permissions.PermissionsGroupRepository(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions)
                == true);
        }
    }
}