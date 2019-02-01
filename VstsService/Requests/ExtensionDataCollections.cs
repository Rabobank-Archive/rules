using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class ExtensionDataCollections
    {
        public static IVstsRestRequest<Response.ExtensionCollectionData> ExtensionData(string publisher, string extensionName, string collection, string id)
        {
            return new ExtmgmtRequest<Response.ExtensionCollectionData>($"_apis/ExtensionManagement/InstalledExtensions/{ publisher }/{ extensionName }/Data/Scopes/Default/Current/Collections/{ collection}/Documents/{id}?api-version=3.1-preview.1");
        }

        public static IVstsPostRequest<Response.ExtensionCollectionData> ExtensionData(string publisher, string extensionName, string collection, string id, object data)
        {
            return new ExtmgmtPutRequest<Response.ExtensionCollectionData>($"_apis/ExtensionManagement/InstalledExtensions/{ publisher }/{ extensionName }/Data/Scopes/Default/Current/Collections/{ collection}/Documents?api-version=3.1-preview.1",new ExtensionData(id, data));
        }
    }

    public class ExtensionData
    {
        public string Id { get; set; }
        public object Data { get; set; }

        public int __etag { get { return -1; }}

        public ExtensionData(string id, object data)
        {
            this.Id = id;
            this.Data = data;
        }
    }
}