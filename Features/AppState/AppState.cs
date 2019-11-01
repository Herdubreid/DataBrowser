using BlazorState;
using DataBrowser.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DataBrowser.Features.AppState
{
    public partial class AppState : State<AppState>
    {
        CloudStorageService CloudStorage { get; }
        public event EventHandler Changed;
        public List<QueryRequest> QueryRequests { get; } = new List<QueryRequest>();
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
        public bool Authenticated => AuthResponse != null;
        public override void Initialize()
        {
            var root = CloudStorage.CloudFileShare.GetRootDirectoryReference();
            foreach (var f in root.ListFilesAndDirectories())
            {
                var source = root.GetFileReference(f.Uri.Segments[f.Uri.Segments.Length - 1]);
                try
                {
                    var t = source.DownloadText();
                    var qr = JsonSerializer.Deserialize<QueryRequest>(t);
                    QueryRequests.Add(qr);
                }
                catch { }
            }
        }
        public AppState(CloudStorageService cloudStorage)
        {
            CloudStorage = cloudStorage;
        }
    }
}
