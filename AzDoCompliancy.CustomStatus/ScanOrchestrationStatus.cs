namespace AzDoCompliancy.CustomStatus
{
    public class ScanOrchestrationStatus : ICustomStatus
    {
        public string Project { get; set; }
        public string Scope { get; set; }
        public string TypeId
        {
            get => TypeIds.ScanOrchestrationStatusId;
        }
    }
}