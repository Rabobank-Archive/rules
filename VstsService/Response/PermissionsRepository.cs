using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class PermissionsRepository
    {
        public IEnumerable<Response.Permission> Permissions { get; set; }
        public string CurrentTeamFoundationId { get; set; }
    }
}