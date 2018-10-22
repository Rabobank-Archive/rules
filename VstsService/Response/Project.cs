namespace SecurePipelineScan.VstsService.Response
{
    public class Project
    {
        public string Name { get; set; }

        public string Id { get; internal set; }

        public string Description { get; set; }

        public string Url { get; set; }
    }
}