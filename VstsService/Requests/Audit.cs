using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class AuditLog
    {
        public static AuditRequest<AuditLogEntries> Query()
        {
            return new AuditRequest<AuditLogEntries>("_apis/audit/auditLog");
        }
    }
}