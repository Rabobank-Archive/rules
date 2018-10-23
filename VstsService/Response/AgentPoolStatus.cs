namespace SecurePipelineScan.VstsService.Response
{
    public class AgentStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string OsDescription { get; set; }

        public bool Enabled { get; set; }

        public string Status { get; set; }

        public AssignedRequest AssignedRequest { get; set; }
    }
}