namespace AzDoCompliancy.CustomStatus
{
    public class SupervisorOrchestrationStatus : CustomStatusBase
    {
        public int TotalProjectCount { get; set; }
        public override string TypeId => TypeIds.SupervisorOrchestrationStatusId;
    }
}