using System;
using System.Collections;
using System.Collections.Generic;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using RestSharp;
using ApplicationGroup = SecurePipelineScan.VstsService.Requests.ApplicationGroup;
using Permission = SecurePipelineScan.Rules.Checks.Permission;
using Project = SecurePipelineScan.VstsService.Requests.Project;
using Release = SecurePipelineScan.VstsService.Requests.Release;
using Repository = SecurePipelineScan.VstsService.Response.Repository;
using SecurityNamespace = SecurePipelineScan.VstsService.Requests.SecurityNamespace;

namespace SecurePipelineScan.Rules
{
    public class SecurityReportScan : IProjectScan<SecurityReport>
    {
        private readonly IVstsRestClient client;

        public SecurityReportScan(IVstsRestClient client)
        {
            this.client = client;
        }

        public SecurityReport Execute(string project, DateTime date)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var applicationGroups =
                client.Get(ApplicationGroup.ApplicationGroups(project))
                    ?.Identities;

            if (applicationGroups == null)
            {
                throw new ArgumentNullException(nameof(applicationGroups));
            }

            var groupMembersProjectAdministrators =
                getGroupMembersOfProjectAdministrators(project, applicationGroups);

            //Todo: can be done outside loop for projects / just once per scan (=organisation)
            var securityNamespaces = client.Get(SecurityNamespace.SecurityNamespaces()).Value;

            if (securityNamespaces == null)
            {
                throw new ArgumentNullException(nameof(securityNamespaces));
            }
            
            var namespaceIdGitRepositories = securityNamespaces
                .FirstOrDefault(ns => ns.DisplayName == "Git Repositories")
                ?.NamespaceId;

            var namespaceIdBuild = securityNamespaces
                .FirstOrDefault(ns => ns.Name == "Build")
                ?.NamespaceId;

            var namespaceIdRelease = securityNamespaces
                .FirstOrDefault(ns => ns.Name == "ReleaseManagement" &&
                                      ns.Actions.Any(action => action.Name.Equals("ViewReleaseDefinition")))
                ?.NamespaceId;



            var applicationGroupIdProjectAdmins = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Project Administrators")
                ?.TeamFoundationId;

            var applicationGroupIdBuildAdmins = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Build Administrators")
                ?.TeamFoundationId;

            var applicationGroupIdReleaseAdmins = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Release Administrators")
                ?.TeamFoundationId;

            var applicationGroupIdProdEnvOwners = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Production Environment Owners")
                ?.TeamFoundationId;

            var applicationGroupRabobankProjectAdministrators = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Rabobank Project Administrators")
                ?.TeamFoundationId;

            var applicationGroupIdContributors = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Contributors")
                ?.TeamFoundationId;


            var projectId = client.Get(Project.Properties(project)).Id;

            var repositories = client.Get(VstsService.Requests.Repository.Repositories(projectId)).Value;

            var buildDefinitions = client.Get(Builds.BuildDefinitions(projectId)).Value;

            var releaseDefinitions = client.Get((Release.Definitions(projectId))).Value;

            
            var permissionsGitRepositorySet =
                applicationGroupIdProjectAdmins != null
                    ? client.Get(Permissions.PermissionsGroupRepositorySet(
                        projectId, namespaceIdGitRepositories, applicationGroupIdProjectAdmins))
                    : null;

            
            var permissionsBuildProjectAdmins =
                applicationGroupIdProjectAdmins != null
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdBuild, applicationGroupIdProjectAdmins))
                    : null;

            var permissionsBuildBuildAdmins =
                applicationGroupIdBuildAdmins != null
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdBuild, applicationGroupIdBuildAdmins))
                    : null;

            var permissionsBuildContributors =
                applicationGroupIdContributors != null
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdBuild, applicationGroupIdContributors))
                    : null;


            var permissionsTeamRabobankProjectAdministrators =
                (applicationGroupRabobankProjectAdministrators != null)
                    ? client.Get(Permissions.PermissionsGroupProjectId(
                        projectId, applicationGroupRabobankProjectAdministrators))
                    : null;

            var permissionsReleaseRabobankProjectAdministrators =
                namespaceIdRelease != null &&
                applicationGroupRabobankProjectAdministrators != null
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupRabobankProjectAdministrators))
                    : null;

            var permissionsReleaseProdEnvOwner =
                applicationGroupIdProdEnvOwners != null
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupIdProdEnvOwners))
                    : null;

            var permissionsReleaseContributors =
                applicationGroupIdContributors != null
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupIdContributors))
                    : null;

            var permissionsReleaseProjectAdministrators =
                applicationGroupIdProjectAdmins != null
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupIdProjectAdmins))
                    : null;

            var permissionsReleaseReleaseAdministrators =
                applicationGroupIdReleaseAdmins != null
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupIdReleaseAdmins))
                    : null;


            var securityReport = new SecurityReport(date)
            {
                Project = project,

                ApplicationGroupContainsProductionEnvironmentOwner =
                    ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(applicationGroups),
                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup =
                    groupMembersProjectAdministrators != null &&
                    ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(
                        groupMembersProjectAdministrators),

                
                RepositoryRightsProjectAdmin =
                    (permissionsGitRepositorySet != null && repositories != null && applicationGroupIdProjectAdmins != null)
                        ? CheckRepositoryRights(permissionsGitRepositorySet.Permissions,
                            repositories, projectId, namespaceIdGitRepositories, applicationGroupIdProjectAdmins, new RepositoryRights())
                        : null,


                BuildRightsBuildAdmin =
                    permissionsBuildBuildAdmins != null
                        ? CheckBuildRights(permissionsBuildBuildAdmins.Permissions, new BuildAdminBuildRights())
                        : null,
                BuildRightsProjectAdmin =
                    permissionsBuildProjectAdmins != null
                        ? CheckBuildRights(permissionsBuildProjectAdmins.Permissions, new ProjectAdminBuildRights())
                        : null,
                BuildRightsContributor =
                    (permissionsBuildContributors != null)
                        ? CheckBuildRights(permissionsBuildContributors.Permissions, new ContributorsBuildRights())
                        : null,

                BuildDefinitionsRightsBuildAdmin =
                    (buildDefinitions != null && applicationGroupIdBuildAdmins != null)
                        ? CheckBuildDefinitionsRights(buildDefinitions, projectId, namespaceIdBuild,
                            applicationGroupIdBuildAdmins, new BuildAdminBuildRights())
                        : null,
                BuildDefinitionsRightsProjectAdmin =
                    (buildDefinitions != null && applicationGroupIdProjectAdmins != null)
                        ? CheckBuildDefinitionsRights(buildDefinitions, projectId, namespaceIdBuild,
                            applicationGroupIdProjectAdmins, new ProjectAdminBuildRights())
                        : null,
                BuildDefinitionsRightsContributor =
                    (buildDefinitions != null && applicationGroupIdContributors != null)
                        ? CheckBuildDefinitionsRights(buildDefinitions, projectId, namespaceIdBuild,
                            applicationGroupIdContributors, new ContributorsBuildRights())
                        : null,


                ReleaseRightsProductionEnvOwner =
                    permissionsReleaseProdEnvOwner != null
                        ? CheckReleaseRights(permissionsReleaseProdEnvOwner.Permissions,
                            new ProductionEnvOwnerReleaseRights())
                        : null,
                ReleaseRightsRaboProjectAdmin =
                    permissionsReleaseRabobankProjectAdministrators != null
                        ? CheckReleaseRights(permissionsReleaseRabobankProjectAdministrators.Permissions,
                            new RaboAdminReleaseRights())
                        : null,
                ReleaseRightsContributor =
                    permissionsReleaseContributors != null
                        ? CheckReleaseRights(permissionsReleaseContributors.Permissions,
                            new ContributorsReleaseRights())
                        : null,
                ReleaseRightsProjectAdmin =
                    permissionsReleaseProjectAdministrators != null
                        ? CheckReleaseRights(permissionsReleaseProjectAdministrators.Permissions,
                            new ProjectAdminReleaseRights())
                        : null,
                ReleaseRightsReleaseAdmin =
                    permissionsReleaseReleaseAdministrators != null
                        ? CheckReleaseRights(permissionsReleaseReleaseAdministrators.Permissions,
                            new ProjectAdminReleaseRights())
                        : null,

                ReleaseDefinitionsRightsProductionEnvOwner =
                    (releaseDefinitions != null && applicationGroupIdProdEnvOwners != null)
                        ? CheckReleaseDefinitionsRights(releaseDefinitions, projectId, namespaceIdRelease,
                            applicationGroupIdProdEnvOwners, new ProductionEnvOwnerReleaseRights())
                        : null,
                ReleaseDefinitionsRightsContributor =
                    (releaseDefinitions != null && applicationGroupIdContributors != null)
                        ? CheckReleaseDefinitionsRights(releaseDefinitions, projectId, namespaceIdRelease,
                            applicationGroupIdContributors, new ContributorsReleaseRights())
                        : null,
                ReleaseDefinitionsRightsRaboProjectAdmin =
                    (releaseDefinitions != null && applicationGroupRabobankProjectAdministrators != null)
                        ? CheckReleaseDefinitionsRights(releaseDefinitions, projectId, namespaceIdRelease,
                            applicationGroupRabobankProjectAdministrators, new RaboAdminReleaseRights())
                        : null,
                ReleaseDefinitionsRightsProjectAdmin =
                    (releaseDefinitions != null && applicationGroupIdProjectAdmins != null)
                        ? CheckReleaseDefinitionsRights(releaseDefinitions, projectId, namespaceIdRelease,
                            applicationGroupIdProjectAdmins, new ProjectAdminReleaseRights())
                        : null,
                ReleaseDefinitionsRightsReleaseAdmin =
                    (releaseDefinitions != null && applicationGroupIdReleaseAdmins != null)
                        ? CheckReleaseDefinitionsRights(releaseDefinitions, projectId, namespaceIdRelease,
                            applicationGroupIdReleaseAdmins, new ReleaseAdminReleaseRights())
                        : null,

                
                TeamRabobankProjectAdministrators =
                    permissionsTeamRabobankProjectAdministrators != null
                        ? CheckTeamRabobankProjectAdministrators(permissionsTeamRabobankProjectAdministrators.Security
                            .Permissions)
                        : null,
            };
            
            return securityReport;
        }

        private GlobalRights CheckTeamRabobankProjectAdministrators(
            IEnumerable<SecurePipelineScan.VstsService.Response.Permission> permissions)
        {
            return new GlobalRights
            {
                HasNoPermissionToDeleteTeamProject =
                    Permission.HasNoPermissionToDeleteTeamProject(permissions),
                HasNoPermissionToPermanentlyDeleteWorkitems =
                    Permission.HasNoPermissionToPermanentlyDeleteWorkitems(permissions),
                HasNoPermissionToManageProjectProperties =
                    Permission.HasNoPermissionToManageProjectProperties(permissions),
            };
        }

        private RepositoryRights CheckRepositoryRights(
            IEnumerable<SecurePipelineScan.VstsService.Response.Permission> permissions,
            IEnumerable<Repository> repositories, string projectId, string namespaceId, 
            string applicationGroupId, RepositoryRights repositoryRights)
        {
            List<PermissionsSetId> permissionsRepositories = new List<PermissionsSetId>();
            
            foreach (var repository in repositories)
            {
                permissionsRepositories.Add(
                    client.Get(Permissions.PermissionsGroupRepository(
                        projectId, namespaceId, applicationGroupId, repository.Id)));
            }

            repositoryRights.HasNoPermissionToDeleteRepositories = true;
            repositoryRights.HasNotSetToManagePermissionsRepositories = true;

            foreach (var permissionsRepository in permissionsRepositories)
            {
                repositoryRights.HasNoPermissionToDeleteRepositories =
                    repositoryRights.HasNoPermissionToDeleteRepositories &&
                    Permission.HasNoPermissionToDeleteRepository(permissionsRepository.Permissions);
                repositoryRights.HasNotSetToManagePermissionsRepositories =
                    repositoryRights.HasNotSetToManagePermissionsRepositories &&
                    Permission.HasNotSetToManageRepositoryPermissions(permissionsRepository.Permissions);
            }

            repositoryRights.HasNoPermissionToDeleteRepositorySet =
                Permission.HasNoPermissionToDeleteRepository(permissions);
            repositoryRights.HasNotSetToManagePermissionsRepositorySet =
                Permission.HasNotSetToManageRepositoryPermissions(permissions);

            return repositoryRights;
        }

        private BuildRights CheckBuildRights(
            IEnumerable<VstsService.Response.Permission> permissions, BuildRights buildRights)
        {
            {
                buildRights.HasNoPermissionsToAdministerBuildPermissions =
                    Permission.HasNoPermissionToAdministerBuildPermissions(permissions);
                buildRights.HasNoPermissionsToDeleteBuilds =
                    Permission.HasNoPermissionToDeleteBuilds(permissions);
                buildRights.HasNoPermissionsToDestroyBuilds =
                    Permission.HasNoPermissionToDestroyBuilds(permissions);
                buildRights.HasNoPermissionsToDeleteBuildDefinition =
                    Permission.HasNoPermissionToDeleteBuildDefinition(permissions);
                buildRights.HasNotSetToDeleteBuildDefinition =
                    Permission.HasNotSetToDeleteBuildDefinition(permissions);
                buildRights.HasNotSetToDeleteBuilds =
                    Permission.HasNotSetToDeleteBuilds(permissions);
                buildRights.HasNotSetToDestroyBuilds =
                    Permission.HasNotSetToDestroyBuilds(permissions);
            };

            return buildRights;
        }

        private ReleaseRights CheckReleaseRights(
            IEnumerable<VstsService.Response.Permission> permissions,
            ReleaseRights releaseRights)
        {
            releaseRights.HasNoPermissionToCreateReleases =
                Permission.HasNoPermissionToCreateReleases(permissions);
            releaseRights.HasPermissionToCreateReleases =
                Permission.HasPermissionToCreateReleases(permissions);
            releaseRights.HasNotSetToManageReleaseApprovers =
                Permission.HasNotSetToManageReleaseApprovers(permissions);
            releaseRights.HasPermissionToManageReleaseApprovers =
                Permission.HasPermissionToManageReleaseApprovers(permissions);
            releaseRights.HasNoPermissionToDeleteReleases =
                Permission.HasNoPermissionToDeleteReleases(permissions);
            releaseRights.HasNoPermissionToManageReleaseApprovers =
                Permission.HasNoPermissionToManageReleaseApprovers(permissions);
            releaseRights.HasNoPermissionsToAdministerReleasePermissions =
                Permission.HasNoPermissionToAdministerReleasePermissions(permissions);
            releaseRights.HasNoPermissionToDeleteReleasePipeline =
                Permission.HasNoPermissionToDeleteReleasePipeline(permissions);
            releaseRights.HasNoPermissionToDeleteReleaseStage =
                Permission.HasNoPermissionToDeleteReleaseStage(permissions);
            releaseRights.HasPermissionToDeleteReleaseStage =
                Permission.HasPermissionToDeleteReleaseStage(permissions);
            releaseRights.HasNotSetToDeleteReleaseStage =
                Permission.HasNotSetToDeleteReleaseStage(permissions);
            return releaseRights;
        }

        private ReleaseRights CheckReleaseDefinitionsRights(
            IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId,
            string applicationGroupId, ReleaseRights releaseRights)
        {
            List<PermissionsSetId> permissionsDefinitions = new List<PermissionsSetId>();

            foreach (var releaseDefinition in releaseDefinitions)
            {
                permissionsDefinitions.Add(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                        projectId, namespaceId, applicationGroupId, releaseDefinition.Id)));
            }

            releaseRights.HasNotSetToManageReleaseApprovers = true;
            releaseRights.HasNoPermissionsToAdministerReleasePermissions = true;
            releaseRights.HasNoPermissionToCreateReleases = true;
            releaseRights.HasNoPermissionToDeleteReleases = true;
            releaseRights.HasNoPermissionToDeleteReleasePipeline = true;
            releaseRights.HasNoPermissionToManageReleaseApprovers = true;
            releaseRights.HasPermissionToManageReleaseApprovers = true;
            releaseRights.HasNoPermissionToDeleteReleaseStage = true;
            releaseRights.HasNotSetToDeleteReleaseStage = true;
            releaseRights.HasPermissionToDeleteReleaseStage = true;

            foreach (var permissionsDefinition in permissionsDefinitions)
            {
                releaseRights.HasNotSetToManageReleaseApprovers =
                    releaseRights.HasNotSetToManageReleaseApprovers &&
                    Permission.HasNotSetToManageReleaseApprovers(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasNoPermissionsToAdministerReleasePermissions =
                    releaseRights.HasNoPermissionsToAdministerReleasePermissions &&
                    Permission.HasNoPermissionToAdministerReleasePermissions(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasNoPermissionToCreateReleases =
                    releaseRights.HasNoPermissionToCreateReleases &&
                    Permission.HasNoPermissionToCreateReleases(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasNoPermissionToDeleteReleases =
                    releaseRights.HasNoPermissionToDeleteReleases &&
                    Permission.HasNoPermissionToDeleteReleases(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasNoPermissionToDeleteReleasePipeline =
                    releaseRights.HasNoPermissionToDeleteReleasePipeline &&
                    Permission.HasNoPermissionToDeleteReleasePipeline(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasNoPermissionToManageReleaseApprovers =
                    releaseRights.HasNoPermissionToManageReleaseApprovers &&
                    Permission.HasNoPermissionToManageReleaseApprovers(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasPermissionToManageReleaseApprovers =
                    releaseRights.HasPermissionToManageReleaseApprovers &&
                    Permission.HasPermissionToManageReleaseApprovers(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasNoPermissionToDeleteReleaseStage =
                    releaseRights.HasNoPermissionToDeleteReleaseStage &&
                    Permission.HasNoPermissionToDeleteReleaseStage(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasNotSetToDeleteReleaseStage =
                    releaseRights.HasNotSetToDeleteReleaseStage &&
                    Permission.HasNotSetToDeleteReleaseStage(
                        permissionsDefinition
                            .Permissions);
                releaseRights.HasPermissionToDeleteReleaseStage =
                    releaseRights.HasPermissionToDeleteReleaseStage &&
                    Permission.HasPermissionToDeleteReleaseStage(
                        permissionsDefinition
                            .Permissions);
            }
            return releaseRights;
        }

        private BuildRights CheckBuildDefinitionsRights(
            IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId,
            string applicationGroupId, BuildRights buildRights)
        {
            List<PermissionsSetId> permissionsDefinitionsBuilds = new List<PermissionsSetId>();

            foreach (var buildDefinition in buildDefinitions)
            {
                var permissionsDefinitionSetId = client.Get(Permissions.PermissionsGroupSetIdDefinition(
                    projectId, namespaceId, applicationGroupId, buildDefinition.Id));

                permissionsDefinitionsBuilds.Add(permissionsDefinitionSetId);
            }

            buildRights.HasNoPermissionsToDeleteBuilds = true;
            buildRights.HasNoPermissionsToDestroyBuilds = true;
            buildRights.HasNoPermissionsToDeleteBuildDefinition = true;
            buildRights.HasNoPermissionsToAdministerBuildPermissions = true;
            buildRights.HasNotSetToDeleteBuildDefinition = true;
            buildRights.HasNotSetToDeleteBuilds = true;
            buildRights.HasNotSetToDestroyBuilds = true;

            foreach (var permissionsDefinition in permissionsDefinitionsBuilds)
            {
                buildRights.HasNoPermissionsToDeleteBuilds =
                    buildRights.HasNoPermissionsToDeleteBuilds && Permission.HasNoPermissionToDeleteBuilds(
                        permissionsDefinition
                            .Permissions);
                buildRights.HasNoPermissionsToDestroyBuilds =
                    buildRights.HasNoPermissionsToDestroyBuilds && Permission.HasNoPermissionToDestroyBuilds(
                        permissionsDefinition
                            .Permissions);
                buildRights.HasNoPermissionsToDeleteBuildDefinition =
                    buildRights.HasNoPermissionsToDeleteBuildDefinition &&
                    Permission.HasNoPermissionToDeleteBuildDefinition(
                        permissionsDefinition
                            .Permissions);
                buildRights.HasNoPermissionsToAdministerBuildPermissions =
                    buildRights.HasNoPermissionsToAdministerBuildPermissions &&
                    Permission.HasNoPermissionToAdministerBuildPermissions(
                        permissionsDefinition
                            .Permissions);
                buildRights.HasNotSetToDeleteBuildDefinition =
                    buildRights.HasNotSetToDeleteBuildDefinition &&
                    Permission.HasNotSetToDeleteBuildDefinition(
                        permissionsDefinition
                            .Permissions);
                buildRights.HasNotSetToDeleteBuilds =
                    buildRights.HasNotSetToDeleteBuilds && Permission.HasNotSetToDeleteBuilds(
                        permissionsDefinition
                            .Permissions);
                buildRights.HasNotSetToDestroyBuilds =
                    buildRights.HasNotSetToDestroyBuilds && Permission.HasNotSetToDestroyBuilds(
                        permissionsDefinition
                            .Permissions);
            }

            return buildRights;
        }

        private IEnumerable<VstsService.Response.ApplicationGroup> getGroupMembersOfProjectAdministrators(
            string project, IEnumerable<VstsService.Response.ApplicationGroup> applicationGroups)
        {
            var groupId = applicationGroups.Single(x => x.DisplayName == $"[{project}]\\Project Administrators")
                .TeamFoundationId;
            return client.Get(ApplicationGroup.GroupMembers(project, groupId)).Identities;
        }
    }
}
        

        



        
        
        
        



