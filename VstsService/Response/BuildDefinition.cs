namespace SecurePipelineScan.VstsService.Response
{
    public class BuildDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BuildProcess Process { get; set; }
        public TeamProjectReference Project { get; set; }
        public Repository Repository { get; set; }
    }
}