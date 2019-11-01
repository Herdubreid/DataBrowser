using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Configuration;

namespace DataBrowser.Services
{
    public class CloudStorageService
    {
        readonly string SHARE = "cql";
        CloudFileClient CloudFileClient { get; }
        public CloudFileShare CloudFileShare => CloudFileClient.GetShareReference(SHARE);
        public CloudStorageService(IConfiguration config)
        {
            var cloudStorage = CloudStorageAccount.Parse(config["azureStorage"]);
            CloudFileClient = cloudStorage.CreateCloudFileClient();
        }
    }
}
