namespace vsts.Response
{
    public class DeployStep
    {
        public Identity RequestedFor { get; set; }
        public Identity LastModifiedBy { get; set; }
    }
}