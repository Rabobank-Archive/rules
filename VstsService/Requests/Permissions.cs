using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Permissions
    {
        public static IVstsRequest<PermissionsSetId> PermissionsGroupRepositorySet(string projectId, string permissionSetId, string applicationGroupId)
        {
            return new VstsRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions", new Dictionary<string, object>
                {
                    {"__v","5"},
                    {"tfid", applicationGroupId},
                    {"permissionSetId", permissionSetId},
                    {"permissionSetToken", $"repoV2/{projectId}"}
                });
        }
        
        public static IVstsRequest<PermissionsSetId> PermissionsGroupRepository(string projectId, string permissionSetId, string applicationGroupId, string repositoryId)
        {
            return new VstsRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions", new Dictionary<string, object>
                {
                    {"__v","5"},
                    {"tfid", applicationGroupId},
                    {"permissionSetId", permissionSetId},
                    {"permissionSetToken", $"repoV2/{projectId}/{repositoryId}"}
                });
        }

        public static IVstsRequest<PermissionsSetId> PermissionsGroupMasterBranch(string projectId, string permissionSetId, string applicationGroupId, string repositoryId)
        {
            return new VstsRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions", new Dictionary<string, object>
                {
                    {"__v","5"},
                    {"tfid", applicationGroupId},
                    {"permissionSetId", permissionSetId},
                    {"permissionSetToken", $"repoV2/{projectId}/{repositoryId}/refs/heads/master"}
                });
        }

        public static IVstsRequest<PermissionsSetId> PermissionsGroupSetId(string projectId,string permissionSetId, string applicationGroupId)
        {
            return new VstsRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions", new Dictionary<string, object>
                {
                    {"__v","5"},
                    {"tfid", applicationGroupId},
                    {"permissionSetId", permissionSetId},
                    {"permissionSetToken", projectId}
                });
        }

        public static IVstsRequest<PermissionsSetId> PermissionsGroupSetIdDefinition(string projectId,
            string permissionSetId, string applicationGroupId, string permissionSetToken)
        {
            return new VstsRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions", new Dictionary<string, object>
                {
                    {"__v","5"},
                    {"tfid", applicationGroupId},
                    {"permissionSetId", permissionSetId},
                    {"permissionSetToken", permissionSetToken}
                });
        }
        
        /// <summary>
        /// Gets project permissions for an applicationGroup
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="applicationGroupId"></param>
        /// <returns></returns>
        public static IVstsRequest<PermissionsProjectId> PermissionsGroupProjectId(string projectId, string applicationGroupId)
        {
            return new VstsRequest<PermissionsProjectId>(
                $"{projectId}/_api/_identity/Display", new Dictionary<string, object>
                {
                    {"__v", "5"},
                    {"tfid", applicationGroupId}
                });
        }

        /// <summary>
        /// But ugly REST API where this wrapper is required and only has one property with JSON serialized content
        /// </summary>
        public class UpdateWrapper
        {
            public string UpdatePackage { get; }

            public UpdateWrapper(string content)
            {
                UpdatePackage = content;
            }
        }

        public static IVstsRequest<UpdateWrapper, object> ManagePermissions(string project) =>
            new VstsRequest<UpdateWrapper, object>($"{project}/_api/_security/ManagePermissions", new Dictionary<string, object>
            {
                {"__v", "5"}
            });
    }
}