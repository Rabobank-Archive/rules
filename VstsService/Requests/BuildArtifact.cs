namespace SecurePipelineScan.VstsService.Requests
{
    public class BuildArtifact
    {
        public int Id { get; set; }
        public ArtifactResource Resource { get; set; }
    }
}