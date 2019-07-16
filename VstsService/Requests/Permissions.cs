using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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
            string permissionSetId, string applicationGroupId, string definitionId)
        {
            return new VstsRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions", new Dictionary<string, object>
                {
                    {"__v","5"},
                    {"tfid", applicationGroupId},
                    {"permissionSetId", permissionSetId},
                    {"permissionSetToken", $"{projectId}/{definitionId}"}
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

        public class ManagePermissionsData
        {
            public ManagePermissionsData(string tfid,
                string descriptorIdentifier,
                string descriptorIdentityType,
                params Permission[] permissions)
            {
                TeamFoundationId = tfid;
                DescriptorIdentityType = descriptorIdentityType;
                DescriptorIdentifier = descriptorIdentifier;
                Updates = permissions.Select(x => new
                {
                    Token = x.PermissionToken,
                    x.PermissionId,
                    x.NamespaceId,
                    x.PermissionBit
                });

                var first = permissions.First();
                PermissionSetId = first.NamespaceId;
                PermissionSetToken = ExtractToken(first.PermissionToken);
            }

            private static string ExtractToken(string token)
            {
                return Regex.Match(token, @"^(?:\$PROJECT:)?(.*?)(?::)?$").Groups[1].Value;
            }

            public object Updates { get; }

            public string TeamFoundationId { get; }
            public string PermissionSetId { get; }
            public string PermissionSetToken { get; }
            public string DescriptorIdentityType { get; }
            public string DescriptorIdentifier { get; }
            public bool RefreshIdentities { get; }
            public string TokenDisplayName { get; }
            
            public UpdateWrapper Wrap() =>
                new UpdateWrapper(JsonConvert.SerializeObject(this));
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