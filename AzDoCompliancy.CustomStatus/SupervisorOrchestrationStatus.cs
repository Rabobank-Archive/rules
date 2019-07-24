namespace AzDoCompliancy.CustomStatus
{
    public class SupervisorOrchestrationStatus : ICustomStatus
    {
        public int TotalProjectCount { get; set; }
        public string TypeId
        {
            get => TypeIds.SupervisorOrchestrationStatusId;
        }
    }
}