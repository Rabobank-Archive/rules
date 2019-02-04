using Newtonsoft.Json;
using RestSharp;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ExtensionManagement
    {
        public static IVstsRestRequest<TExtensionData> ExtensionData<TExtensionData>(
            string publisher,
            string extensionName, 
            string collection, 
            string id) where TExtensionData: ExtensionData, new()
        {
            return new ExtmgmtRequest<TExtensionData>(
                $"_apis/ExtensionManagement/InstalledExtensions/{publisher}/{extensionName}/Data/Scopes/Default/Current/Collections/{collection}/Documents/{id}?api-version=3.1-preview.1");
        }

        public static IVstsRestRequest<TExtensionData> ExtensionData<TExtensionData>(
            string publisher,
            string extensionName, 
            string collection) where TExtensionData: ExtensionData, new()
        {
            return new ExtmgmtRequest<TExtensionData>(
                $"_apis/ExtensionManagement/InstalledExtensions/{publisher}/{extensionName}/Data/Scopes/Default/Current/Collections/{collection}/Documents?api-version=3.1-preview.1");
        }
    }
}