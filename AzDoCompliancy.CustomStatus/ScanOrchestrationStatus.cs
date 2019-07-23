namespace AzDoCompliancy.CustomStatus
{
    public class ScanOrchestrationStatus : CustomStatus
    {
        public string Project { get; set; }
        public string Scope { get; set; }
        public override string TypeId
        {
            get => TypeIds.ScanOrchestrationStatusId;
        }
    }
}