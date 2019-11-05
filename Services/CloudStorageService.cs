using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Configuration;

namespace DataBrowser.Services
{
    public enum StorageType
    {
        Contexts,
        Requests,
        Responses
    }
    public class CloudStorageService
    {
        readonly string SHARE = "cql";
        readonly string CONTEXTS = "contexts";
        readonly string REQUESTS = "requests";
        readonly string RESPONSES = "responses";
        CloudFileClient CloudFileClient { get; }
        public CloudFileShare CloudFileShare => CloudFileClient.GetShareReference(SHARE);
        public CloudFileDirectory RequestsDirectory
        {
            get
            {
                var root = CloudFileShare.GetRootDirectoryReference();
                return root.GetDirectoryReference(REQUESTS);
            }
        }
        public CloudFileDirectory ResponsesDirectory
        {
            get
            {
                var root = CloudFileShare.GetRootDirectoryReference();
                return root.GetDirectoryReference(RESPONSES);
            }
        }
        public CloudFileDirectory ContextDirectory
        {
            get
            {
                var root = CloudFileShare.GetRootDirectoryReference();
                return root.GetDirectoryReference(CONTEXTS);
            }
        }
        public CloudStorageService(IConfiguration config)
        {
            var cloudStorage = CloudStorageAccount.Parse(config["azureStorage"]);
            CloudFileClient = cloudStorage.CreateCloudFileClient();
        }
    }
}
