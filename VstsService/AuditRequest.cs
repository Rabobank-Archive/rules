using System;

namespace SecurePipelineScan.VstsService
{
    public class AuditRequest<T> : VstsRequest<T> where T: new()
    {
        public AuditRequest(string resource) : base(resource)
        {
        }

        public override Uri BaseUri(string organization) => new Uri($"https://auditservice.dev.azure.com/{organization}");
    }
}