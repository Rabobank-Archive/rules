namespace SecurePipelineScan.VstsService.Response
{
    public class AccessLevel
    {
        public string LicensingSource { get; set; }
        public string AccountLicenseType { get; set; }
        public string MsdnLicenseType { get; set; }
        public string LicenseDisplayName { get; set; }
        public string Status { get; set; }
    }
}