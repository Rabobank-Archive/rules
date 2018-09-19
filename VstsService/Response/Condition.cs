namespace SecurePipelineScan.VstsService.Response
{
    public class Condition
    {
        public bool Result { get; set; }
        public string ConditionType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}