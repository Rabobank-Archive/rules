namespace Subscriptions.Console
{
    public class ProjectInfo
    {
        public string Id { get; set; }
        public bool BuildComplete { get; internal set; }
        public bool GitPullRequestCreated { get; internal set; }
        public bool GitPushed { get; internal set; }
        public bool ReleaseDeploymentCompleted { get; internal set; }
    }
}