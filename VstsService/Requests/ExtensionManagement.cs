using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ExtensionManagement
    {
        public static IVstsRequest<TExtensionData> ExtensionData<TExtensionData>(
            string publisher,
            string extensionName,
            string collection,
            string id) where TExtensionData : ExtensionData, new()
        {
            return new ExtmgmtRequest<TExtensionData>(
                $"_apis/ExtensionManagement/InstalledExtensions/{publisher}/{extensionName}/Data/Scopes/Default/Current/Collections/{collection}/Documents/{id}",
                new Dictionary<string, string>
                {
                    {"api-version", "3.1-preview.1"}
                });
        }

        public static IVstsRequest<TExtensionData> ExtensionData<TExtensionData>(
            string publisher,
            string extensionName,
            string collection) where TExtensionData : ExtensionData, new()
        {
            return new ExtmgmtRequest<TExtensionData>(
                $"_apis/ExtensionManagement/InstalledExtensions/{publisher}/{extensionName}/Data/Scopes/Default/Current/Collections/{collection}/Documents",
                new Dictionary<string, string>
                {
                    {"api-version", "3.1-preview.1"}
                });
        }
    }
}