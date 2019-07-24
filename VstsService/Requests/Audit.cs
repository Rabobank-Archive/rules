using System;
using System.Collections.Generic;
using SecurePipelineScan.VstsService.Enumerators;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class AuditLog
    {
        public static IEnumerableRequest<AuditLogEntry> Query(DateTime? start = null, DateTime? end = null)
        {
            return new EnumerableRequest<AuditLogEntry, AuditLogEnumerator>(new AuditRequest<AuditLogEntry>("_apis/audit/auditLog", new Dictionary<string, object>
            {
                ["startTime"] = start,
                ["endTime"] = end
            }));
        }
    }
}