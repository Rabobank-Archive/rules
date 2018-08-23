using Domain;

namespace VstsService
{
    public interface IVstsScanner
    {
        ProjectScanResult ScanProject(string projectName);
        
    }
}