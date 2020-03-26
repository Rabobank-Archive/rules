namespace VstsService.Response
{
    public class YamlPipelineResponse
    {
        public Pipeline Pipeline { get; set; }
        public string FinalYaml { get; set; }
    }
}