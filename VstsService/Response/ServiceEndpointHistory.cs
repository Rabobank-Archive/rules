namespace SecurePipelineScan.VstsService.Response
{
    public class ServiceEndpointHistory
    {
        public ServiceEndpointHistoryData Data { get; set; }
    }

    public class ServiceEndpointHistoryData
    {
        public int Id { get; set; }
        public ReleaseDefinition Definition { get; set; }
        public Owner Owner { get; set; }
        public string PlanType { get; set; }
    }
}