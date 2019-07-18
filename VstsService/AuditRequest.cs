using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class AuditRequest<T> : VstsRequest<T> where T: new()
    {
        public AuditRequest(string resource) : base(resource)
        {
        }

        public AuditRequest(string resource, Dictionary<string, object> queryParams) : base(resource, queryParams)
        {
        }

        public override Uri BaseUri(string organization) => new Uri($"https://auditservice.dev.azure.com/{organization}");
    }
}