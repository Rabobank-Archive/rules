namespace SecurePipelineScan.VstsService.Response
{
    public class Artifact
    {
        public string Alias { get; set; }
        public BuildDefinitionReference DefinitionReference { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsRetained { get; set; }
        public string SourceId { get; set; }
        public string Type { get; set; }
    }
}