namespace AzDoCompliancy.CustomStatus
{
    public class CustomStatusBase
    {
        // Throws intentionally, as this should never be accessed (only from subclasses)
        public virtual string TypeId => throw new System.NotImplementedException();
    }
}