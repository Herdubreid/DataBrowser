using DataBrowser.Services;
using Microsoft.Azure.Storage.File;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.Features
{
    using Fs = Celin.AIS.Form<Celin.AIS.FormData<JsonElement>>;
    public class QueryResponseData<T>
    {
        public IEnumerable<T> rowset { get; set; }
        public IEnumerable<T> output { get; set; }
    }
    public class SaveQueryResponse
    {
        public Guid Id { get; set; }
        public string Query { get; set; }
        public string Title { get; set; }
        public IDictionary<string, string> Columns { get; set; }
        public string Environment { get; set; }
        public DateTime Submitted { get; set; }
        public Celin.AIS.Summary Summary { get; set; }
        public int Count { get; set; }
    }
    public class QueryResponse : SaveQueryResponse
    {
        public string Error { get; set; }
        public bool Busy { get; set; }
        public async Task Submit(E1Service e1, Guid id, Celin.AIS.DatabrowserRequest request, EventHandler handler, CloudFileDirectory fileDirectory)
        {
            try
            {
                Busy = true;
                Title = "Working...";
                Environment = e1.AuthResponse.environment;
                Submitted = DateTime.Now;
                handler?.Invoke(this, null);
                var cancel = new CancellationTokenSource();
                e1.CancelTokens.Add(id, cancel);
                var result = await e1.RequestAsync<JsonElement>(request, cancel);
                var it = result.EnumerateObject();
                QueryResponseData<JsonElement> data = null;
                if (it.MoveNext())
                {
                    var n = it.Current.Name;
                    if (n.StartsWith("fs_"))
                    {
                        Title = it.Current.Value.GetProperty("title").GetString();
                        var fm = JsonSerializer.Deserialize<Fs>(it.Current.Value.ToString());
                        Columns = fm.data.gridData.columns;
                        Summary = fm.data.gridData.summary;
                        Count = Summary.records;
                        data = new QueryResponseData<JsonElement> { rowset = fm.data.gridData.rowset };
                    }
                    else if (n.StartsWith("ds_"))
                    {
                        Title = n;
                        var ds = JsonSerializer.Deserialize<Celin.AIS.Output<JsonElement>>(it.Current.Value.ToString());
                        Count = ds.output.Length;
                        if (Count > 0)
                        {
                            Columns = new Dictionary<string, string>();
                            foreach (var c in ds.output[0].EnumerateObject())
                            {
                                if (c.Value.ValueKind == JsonValueKind.Object)
                                {
                                    foreach (var sc in c.Value.EnumerateObject())
                                    {
                                        var s = $"{c.Name}.{sc.Name}";
                                        Columns.Add(s, s);
                                    }
                                }
                                else
                                {
                                    Columns.Add(c.Name, c.Name);
                                }
                            }
                        }
                        data = new QueryResponseData<JsonElement> { output = ds.output };
                    }
                    else if (n.CompareTo("sysErrors") == 0)
                    {
                        var errs = JsonSerializer.Deserialize<IEnumerable<Celin.AIS.ErrorWarning>>(it.Current.Value.ToString());
                        foreach (var e in errs)
                        {
                            Error = e.DESC;
                        }
                    }
                    if (Count > 0)
                    {
                        var dest = fileDirectory.GetFileReference(Id.ToString());
                        await dest.UploadTextAsync(JsonSerializer.Serialize(this, typeof(SaveQueryResponse),
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            }));
                        var destData = fileDirectory.GetDirectoryReference("data").GetFileReference(Id.ToString());
                        await destData.UploadTextAsync(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
                    }
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
            finally
            {
                Busy = false;
                handler?.Invoke(this, null);
                e1.CancelTokens.Remove(id);
            }
        }
    }
}
