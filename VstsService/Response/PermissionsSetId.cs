using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class PermissionsSetId
    {
        public IEnumerable<Permission> Permissions { get; set; }
        public string CurrentTeamFoundationId { get; set; }
        public string DescriptorIdentifier { get; set; }
        public string DescriptorIdentityType { get; set; }
    }
}