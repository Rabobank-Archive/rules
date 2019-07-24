using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class AuditLogEntry
    {
        public string ActionId { get; set; }
        public string Details { get; set; }
        public string Area { get; set; }
        public string Category { get; set; }
        public string ProjectId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}