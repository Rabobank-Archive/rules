using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Permissions
    {
        public static IVstsRestRequest<Response.PermissionsSetId> PermissionsGroupRepositorySet(string projectId, string permissionSetId, string applicationGroupId)
        {
            return new VstsRestRequest<Response.PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken=repoV2%2F{projectId}");
        }
        
        public static IVstsRestRequest<Response.PermissionsSetId> PermissionsGroupRepository(string projectId, string permissionSetId, string applicationGroupId, string repositoryId)
        {
            return new VstsRestRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken=repoV2%2F{projectId}%2F{repositoryId}");
        }

        public static IVstsRestRequest<Response.PermissionsSetId> PermissionsGroupSetId(string projectId,string permissionSetId, string applicationGroupId)
        {
            return new VstsRestRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken={projectId}");
        }

        public static IVstsRestRequest<Response.PermissionsSetId> PermissionsGroupSetIdDefinition(string projectId,
            string permissionSetId, string applicationGroupId, string definitionId)
        {
            return new VstsRestRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken={projectId}%2F{definitionId}");

        }

        /// <summary>
        /// Gets project permissions for an applicationGroup
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="applicationGroupId"></param>
        /// <returns></returns>
        public static IVstsRestRequest<PermissionsProjectId> PermissionsGroupProjectId(string projectId, string applicationGroupId)
        {
            return new VstsRestRequest<PermissionsProjectId>(
                $"{projectId}/_api/_identity/Display?__v=5&tfid={applicationGroupId}");
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
                return Regex.Match(token, @"(?<=\$PROJECT:).*(?=:)").Value;
            }

            public object Updates { get; }

            public string TeamFoundationId { get; }
            public string PermissionSetId { get; }
            public string PermissionSetToken { get; }
            public string DescriptorIdentityType { get; }
            public string DescriptorIdentifier { get; }
            public bool RefreshIdentities { get; } = false;
            public string TokenDisplayName { get; } = null;
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

        public static IVstsPostRequest<object> ManagePermissions(string project, ManagePermissionsData data) =>
            new VstsPostRequest<object>($"{project}/_api/_security/ManagePermissions?__v=5", new UpdateWrapper(JsonConvert.SerializeObject(data)));
    }
    
}