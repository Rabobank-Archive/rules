using SecurePipelineScan.VstsService.Response;
using System.Linq;
using Newtonsoft.Json;
using static SecurePipelineScan.VstsService.Requests.Permissions;

namespace SecurePipelineScan.VstsService.Permissions
{
    public class ManagePermissionsData
    {
        public ManagePermissionsData(string tfid, string descriptorIdentifier,
            string descriptorIdentityType, string token, params Permission[] permissions)
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
            PermissionSetToken = token;
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
}