namespace AzDoCompliancy.CustomStatus
{
    public class SupervisorOrchestrationStatus : CustomStatus
    {
        public int TotalProjectCount { get; set; }
        public override string TypeId
        {
            get => TypeIds.SupervisorOrchestrationStatusId;
        }
    }
}