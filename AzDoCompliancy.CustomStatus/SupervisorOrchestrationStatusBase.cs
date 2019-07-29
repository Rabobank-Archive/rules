namespace AzDoCompliancy.CustomStatus
{
    public class SupervisorOrchestrationStatusBase : CustomStatusBase
    {
        public int TotalProjectCount { get; set; }
        public override string TypeId => TypeIds.SupervisorOrchestrationStatusId;
    }
}