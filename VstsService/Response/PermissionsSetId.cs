using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class PermissionsSetId
    {
        public IEnumerable<Permission> Permissions { get; set; }
        public string CurrentTeamFoundationId { get; set; }
    }
}