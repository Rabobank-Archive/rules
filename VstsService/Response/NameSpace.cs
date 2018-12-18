namespace SecurePipelineScan.VstsService.Response
{
    public class NameSpace
    {
        public string Name { get; set; }

        public string namespaceId { get; set; }
        
        public string DisplayName { get; set; }
        
        public string ReadPermission { get; set; }
        
        public string WritePermission { get; set; }
    }
}