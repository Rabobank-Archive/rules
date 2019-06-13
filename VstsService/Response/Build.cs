namespace SecurePipelineScan.VstsService.Response
{
    public class Build
    {
        public Project Project { get; set; }
        public int Id { get; set; }
        public Definition Definition { get; set; }
        public string Result { get; set; }
    }
}