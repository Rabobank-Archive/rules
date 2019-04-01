using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Requests
{
    public static partial class ExtensionManagement
    {
        public static IVstsRestRequest<TExtensionData> ExtensionData<TExtensionData>(
            string publisher,
            string extensionName,
            string collection,
            string id) where TExtensionData : ExtensionData, new()
        {
            return new ExtmgmtRequest<TExtensionData>(
                $"_apis/ExtensionManagement/InstalledExtensions/{publisher}/{extensionName}/Data/Scopes/Default/Current/Collections/{collection}/Documents/{id}?api-version=3.1-preview.1");
        }

        public static IVstsRestRequest<TExtensionData> ExtensionData<TExtensionData>(
            string publisher,
            string extensionName,
            string collection) where TExtensionData : ExtensionData, new()
        {
            return new ExtmgmtRequest<TExtensionData>(
                $"_apis/ExtensionManagement/InstalledExtensions/{publisher}/{extensionName}/Data/Scopes/Default/Current/Collections/{collection}/Documents?api-version=3.1-preview.1");
        }

        public class Permission
        {
            public string Bit { get; set; }
            public int ActualValue { get; set; }
            public int ShouldBe { get; set; }
            public bool IsCompliant { get; set; }
        }

        public class ApplicationGroup
        {
            public string name { get; set; }
            public bool isCompliant { get; set; }
            public IEnumerable<Permission> permissions { get; set; }
        }

        public class Namespace
        {
            public string key { get; set; }
            public string name { get; set; }
            public string description { get; set; }

            public bool isCompliant { get; set; }

            public IEnumerable<ApplicationGroup> applicationGroups { get; set; }
        }
    }
}