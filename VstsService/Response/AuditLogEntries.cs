namespace SecurePipelineScan.VstsService.Response
{
    public class AuditLogEntries
    {
        public AuditLogEntry[] DecoratedAuditLogEntries { get; set; }
        public string ContinuationToken { get; set; }
    }
}