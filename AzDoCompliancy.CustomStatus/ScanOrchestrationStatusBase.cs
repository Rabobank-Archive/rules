namespace AzDoCompliancy.CustomStatus
{
    public class ScanOrchestrationStatusBase : CustomStatusBase
    {
        public string Project { get; set; }
        public string Scope { get; set; }
        public override string TypeId => TypeIds.ScanOrchestrationStatusId;
    }
}