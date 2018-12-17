using System.Collections.Generic;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Reports
{
    public class ReleaseReport
    {
        public Response.Release Release { get; set; }
        public IEnumerable<Response.ServiceEndpoint> Endpoints { get; set; }
        public bool IsApproved { get; set; }
    }
}