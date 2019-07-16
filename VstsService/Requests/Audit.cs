using System;
using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class AuditLog
    {
        public static AuditRequest<AuditLogEntries> Query(DateTime? start = null, DateTime? end = null)
        {
            return new AuditRequest<AuditLogEntries>("_apis/audit/auditLog", new Dictionary<string, object>
            {
                ["startTime"] = start,
                ["endTime"] = end
            });
        }
    }
}