﻿using BlazorState;
using DataBrowser.Services;
using Microsoft.Azure.Storage.File;
using OfficeOpenXml;
using Pidgin;
using static Pidgin.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Unit = MediatR.Unit;

namespace DataBrowser.Features.AppState
{
    public partial class AppState
    {
        public class ResponseDataDownloadHandler : ActionHandler<ResponseDataDownloadAction>
        {
            AppState State => Store.GetState<AppState>();
            JsService Js { get; }
            CloudStorageService CloudStorage { get; }
            public override async Task<Unit> Handle(ResponseDataDownloadAction aAction, CancellationToken aCancellationToken)
            {
                var source = CloudStorage.ResponsesDirectory.GetDirectoryReference("data").GetFileReference(aAction.DataId.ToString());
                if (source.Exists())
                {
                    var rsp = State.QueryResponses.Find(r => r.Id.CompareTo(aAction.DataId) == 0);
                    var data = JsonSerializer.Deserialize<QueryResponseData<object[]>>(
                        await source.DownloadTextAsync(),
                        new JsonSerializerOptions
                        {
                            Converters = { new ObjectArrayConverter() }
                        });
                    using var xl = new ExcelPackage();
                    var ws = xl.Workbook.Worksheets.Add("Data");
                    var rows = data.output ?? data.rowset;
                    var header = new List<string[]>
                    {
                        new string[] { "Title:", rsp.Title },
                        new string[] { "Query:", rsp.Query },
                        new string[] { "Environment:", rsp.Environment },
                        new string[] { "Submitted:", rsp.Submitted.ToString() },
                        rsp.Columns.ToArray()
                    };
                    ws.Cells["A1:B5"].LoadFromArrays(header);
                    ws.Cells["A6:A6"].LoadFromArrays(rows);
                    Js.SaveAs("data.xlsx", xl.GetAsByteArray());
                }

                return Unit.Value;
            }
            public ResponseDataDownloadHandler(IStore store, JsService js, CloudStorageService cloudStorage) : base(store)
            {
                Js = js;
                CloudStorage = cloudStorage;
            }
        }
        public class ResponseDataHandler : ActionHandler<ResponseDataAction>
        {
            AppState State => Store.GetState<AppState>();
            CloudStorageService CloudStorage { get; }
            public override Task<Unit> Handle(ResponseDataAction aAction, CancellationToken aCancellationToken)
            {
                switch (aAction.Action)
                {
                    case DataAction.VIEW:
                        if (!State.ResponseData.Contains(aAction.DataId))
                            State.ResponseData.Insert(0, aAction.DataId);
                        break;
                    case DataAction.REMOVE:
                        State.ResponseData.Remove(aAction.DataId);
                        break;
                    case DataAction.CLEAR:
                        State.QueryResponses.Remove(State.QueryResponses.Find(r => r.Id.CompareTo(aAction.DataId) == 0));
                        break;
                    case DataAction.DELETE:
                        State.ResponseData.Remove(aAction.DataId);
                        State.QueryResponses.Remove(State.QueryResponses.Find(r => r.Id.CompareTo(aAction.DataId) == 0));
                        var source = CloudStorage.ResponsesDirectory.GetFileReference(aAction.DataId.ToString());
                        if (source.Exists())
                        {
                            source.DeleteAsync();
                        }
                        var data = CloudStorage.ResponsesDirectory.GetDirectoryReference("data").GetFileReference(aAction.DataId.ToString());
                        if (data.Exists())
                        {
                            data.DeleteAsync();
                        }
                        break;
                }

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public ResponseDataHandler(IStore store, CloudStorageService cloudStorage) : base(store)
            {
                CloudStorage = cloudStorage;
            }
        }
        public class DemoDataHandler : ActionHandler<DemoDataAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(DemoDataAction aAction, CancellationToken aCancellationToken)
            {
                switch (aAction.Action)
                {
                    case DataAction.VIEW:
                        if (!State.DemoData.Contains(aAction.DataId))
                            State.DemoData.Insert(0, aAction.DataId);
                        break;
                    case DataAction.REMOVE:
                        State.DemoData.Remove(aAction.DataId);
                        break;
                }

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public DemoDataHandler(IStore store) : base(store) { }
        }
        public class AddE1ContextHandler : ActionHandler<AddE1ContextAction>
        {
            AppState State => Store.GetState<AppState>();
            CloudStorageService CloudStorage { get; }
            public override Task<Unit> Handle(AddE1ContextAction aAction, CancellationToken aCancellationToken)
            {
                var ct = new E1Context { Name = aAction.Name, BaseUrl = aAction.BaseUrl };
                State.E1Contexts.Add(ct);
                var dest = CloudStorage.ContextDirectory.GetFileReference(ct.Name);
                dest.UploadText(JsonSerializer.Serialize(ct, typeof(SaveE1Context), new JsonSerializerOptions { WriteIndented = true }));

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public AddE1ContextHandler(IStore store, CloudStorageService cloudStorage) : base(store)
            {
                CloudStorage = cloudStorage;
            }
        }
        public class SaveQueryRequestHandler : ActionHandler<SaveQueryRequestAction>
        {
            AppState State => Store.GetState<AppState>();
            CloudStorageService CloudStorage { get; }
            public override Task<Unit> Handle(SaveQueryRequestAction aAction, CancellationToken aCancellationToken)
            {
                var qr = State.QueryRequests.Find(qr => qr.Id.CompareTo(aAction.Id) == 0);
                qr.Title = aAction.Title ?? qr.Title;
                qr.Query = aAction.Query ?? qr.Query;
                var dest = CloudStorage.RequestsDirectory.GetFileReference(qr.Id.ToString());
                dest.UploadTextAsync(JsonSerializer.Serialize(qr, typeof(SaveQueryRequest), new JsonSerializerOptions { WriteIndented = true }));

                return Unit.Task;
            }
            public SaveQueryRequestHandler(IStore store, CloudStorageService cloudStorage) : base(store)
            {
                CloudStorage = cloudStorage;
            }
        }
        public class DeleteQueryRequestHandler : ActionHandler<DeleteQueryRequestAction>
        {
            AppState State => Store.GetState<AppState>();
            CloudStorageService CloudStorage { get; }
            public override Task<Unit> Handle(DeleteQueryRequestAction aAction, CancellationToken aCancellationToken)
            {
                var qr = State.QueryRequests.Find(qr => qr.Id.CompareTo(aAction.Id) == 0);
                var source = CloudStorage.RequestsDirectory.GetFileReference(qr.Id.ToString());
                if (source.Exists())
                {
                    source.DeleteAsync();
                }
                State.QueryRequests.Remove(qr);

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public DeleteQueryRequestHandler(IStore store, CloudStorageService cloudStorage) : base(store)
            {
                CloudStorage = cloudStorage;
            }
        }
        public class AddNewQueryRequestHandler : ActionHandler<AddNewQueryRequestAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(AddNewQueryRequestAction aAction, CancellationToken aCancellationToken)
            {
                var qr = new QueryRequest
                {
                    Id = Guid.NewGuid(),
                    Visible = true,
                    Title = aAction.Title ?? $"New-{State.QueryRequests.Count}",
                    Query = aAction.Query ?? string.Empty,
                    Toggled = DateTime.Now
                };
                State.QueryRequests.Add(qr);

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public AddNewQueryRequestHandler(IStore store) : base(store) { }
        }
        public class ToggleQueryRequestVisibilityHandler : ActionHandler<ToggleQueryRequestVisibilityAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(ToggleQueryRequestVisibilityAction aAction, CancellationToken aCancellationToken)
            {
                var qr = State.QueryRequests.Find(qr => qr.Id.CompareTo(aAction.Id) == 0);
                qr.Visible = !qr.Visible;
                qr.Toggled = DateTime.Now;

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public ToggleQueryRequestVisibilityHandler(IStore store) : base(store) { }
        }
        public class SubmitQueryHandler : ActionHandler<SubmitQueryAction>
        {
            AppState State => Store.GetState<AppState>();
            JsService Js { get; }
            E1Service E1 { get; }
            CloudStorageService CloudStorage { get; }
            public override async Task<Unit> Handle(SubmitQueryAction aAction, CancellationToken aCancellationToken)
            {
                EventHandler handler = State.Changed;
                var qr = State.QueryRequests.Find(qr => qr.Id.CompareTo(aAction.Id) == 0);
                qr.Query = await Js.GetEditorTextAsync(qr.Id.ToString());
                if (qr.Query.Length > 0)
                {
                    var rsp = new QueryResponse
                    {
                        Id = Guid.NewGuid(),
                        Query = qr.Query,
                        Busy = true,
                        Title = "Working...",
                        Environment = E1.AuthResponse.environment,
                        Submitted = DateTime.Now
                    };
                    try
                    {
                        var request = Celin.AIS.Data.DataRequest.Parser.Before(Char(';')).ParseOrThrow(qr.Query.TrimEnd() + ';');
                        rsp.Demo = request.formServiceDemo != null;
                        State.QueryResponses.Insert(0, rsp);
                        State.InRequest = true;
                        handler?.Invoke(this, null);
                        var cancel = new CancellationTokenSource();
                        E1.CancelTokens.Add(rsp.Id, cancel);
                        var result = await E1.RequestAsync<JsonElement>(request, cancel);
                        rsp = State.QueryResponses.Find(r => r.Id.CompareTo(rsp.Id) == 0);
                        var it = result.EnumerateObject();
                        QueryResponseData<JsonElement> data = null;
                        if (it.MoveNext())
                        {
                            var n = it.Current.Name;
                            if (n.StartsWith("fs_"))
                            {
                                rsp.Title = it.Current.Value.GetProperty("title").GetString();
                                var fm = JsonSerializer.Deserialize<Celin.AIS.Form<Celin.AIS.FormData<JsonElement>>>(it.Current.Value.ToString());
                                rsp.Summary = fm.data.gridData.summary;
                                rsp.Count = rsp.Summary.records;
                                if (rsp.Count > 0)
                                {
                                    rsp.Columns = new List<string>();
                                    foreach (var c in fm.data.gridData.rowset[0].EnumerateObject()) rsp.Columns.Add(c.Name);
                                    data = new QueryResponseData<JsonElement> { rowset = fm.data.gridData.rowset };
                                }
                            }
                            else if (n.StartsWith("ds_"))
                            {
                                rsp.Title = n;
                                var ds = JsonSerializer.Deserialize<Celin.AIS.Output<JsonElement>>(it.Current.Value.ToString());
                                if (ds.error != null)
                                {
                                    rsp.Error = ds.error.message;
                                }
                                else
                                {
                                    rsp.Count = ds.output.Length;
                                    if (rsp.Count > 0)
                                    {
                                        rsp.Columns = new List<string>();
                                        foreach (var c in ds.output[0].EnumerateObject())
                                        {
                                            if (c.Value.ValueKind == JsonValueKind.Object)
                                            {
                                                foreach (var sc in c.Value.EnumerateObject())
                                                {
                                                    rsp.Columns.Add($"{c.Name}.{sc.Name}");
                                                }
                                            }
                                            else
                                            {
                                                rsp.Columns.Add(c.Name);
                                            }
                                        }
                                        data = new QueryResponseData<JsonElement> { output = ds.output };
                                    }
                                }
                            }
                            else if (n.CompareTo("sysErrors") == 0)
                            {
                                var errs = JsonSerializer.Deserialize<IEnumerable<Celin.AIS.ErrorWarning>>(it.Current.Value.ToString());
                                foreach (var e in errs)
                                {
                                    rsp.Error = e.DESC;
                                }
                            }
                            if (rsp.Count > 0)
                            {
                                var dest = CloudStorage.ResponsesDirectory.GetFileReference(rsp.Id.ToString());
                                await dest.UploadTextAsync(JsonSerializer.Serialize(rsp,
                                    new JsonSerializerOptions
                                    {
                                        WriteIndented = true
                                    }));
                                var destData = CloudStorage.ResponsesDirectory.GetDirectoryReference("data").GetFileReference(rsp.Id.ToString());
                                await destData.UploadTextAsync(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
                            }
                        }
                    }
                    catch (ParseException e)
                    {
                        Js.SetJsonText(qr.Id.ToString(), e.Message);
                    }
                    catch (Exception e)
                    {
                        rsp = State.QueryResponses.Find(r => r.Id.CompareTo(rsp.Id) == 0);
                        rsp.Error = e.Message;
                    }
                    finally
                    {
                        rsp.Busy = false;
                        State.InRequest = false;
                        handler?.Invoke(State, null);
                    }
                }

                return Unit.Value;
            }
            public SubmitQueryHandler(IStore store, JsService jsService, E1Service e1Service, CloudStorageService cloudStorage) : base(store)
            {
                Js = jsService;
                E1 = e1Service;
                CloudStorage = cloudStorage;
            }
        }
        public class ValidateRequestHandler : ActionHandler<ValidateRequestAction>
        {
            AppState State => Store.GetState<AppState>();
            JsService Js { get; }
            CloudStorageService CloudStorage { get; }
            public override async Task<Unit> Handle(ValidateRequestAction aAction, CancellationToken aCancellationToken)
            {
                var qr = State.QueryRequests.Find(qr => qr.Id.CompareTo(aAction.Id) == 0);
                qr.Query = await Js.GetEditorTextAsync(aAction.Id.ToString());
                if (qr.Query.Length > 0)
                {
                    try
                    {
                        var result = Celin.AIS.Data.DataRequest.Parser.Before(Parser.Char(';')).ParseOrThrow(qr.Query.TrimEnd() + ';');
                        Js.SetJsonText(aAction.Id.ToString(), JsonSerializer.Serialize(result, new JsonSerializerOptions
                        {
                            IgnoreNullValues = true,
                            WriteIndented = true
                        }));
                        var dest = CloudStorage.RequestsDirectory.GetFileReference(qr.Id.ToString());
                        await dest.UploadTextAsync(JsonSerializer.Serialize(qr, typeof(SaveQueryRequest), new JsonSerializerOptions { WriteIndented = true }));
                    }
                    catch (ParseException e)
                    {
                        Js.SetJsonText(aAction.Id.ToString(), e.Message);
                    }
                }
                return Unit.Value;
            }
            public ValidateRequestHandler(IStore store, JsService jsService, CloudStorageService cloudStorage) : base(store)
            {
                Js = jsService;
                CloudStorage = cloudStorage;
            }
        }
        public class RefreshCloudStorageHandler : ActionHandler<RefreshCloudStorageAction>
        {
            AppState State => Store.GetState<AppState>();
            CloudStorageService CloudStorage { get; }
            public override async Task<Unit> Handle(RefreshCloudStorageAction aAction, CancellationToken aCancellationToken)
            {
                switch (aAction.StorageType)
                {
                    case StorageType.Contexts:
                        var contexts = CloudStorage.ContextDirectory;
                        foreach (var f in contexts.ListFilesAndDirectories().OfType<CloudFile>())
                        {
                            var source = contexts.GetFileReference((f as CloudFile).Name);
                            try
                            {
                                var t = await source.DownloadTextAsync();
                                var newCt = JsonSerializer.Deserialize<E1Context>(t);
                                if (!State.E1Contexts.Exists(ct => ct.Name.CompareTo(newCt.Name) == 0))
                                {
                                    State.E1Contexts.Add(newCt);
                                }
                            }
                            catch { }
                        }
                        break;
                    case StorageType.Requests:
                        var requests = CloudStorage.RequestsDirectory;
                        foreach (var f in requests.ListFilesAndDirectories().OfType<CloudFile>())
                        {
                            var source = requests.GetFileReference((f as CloudFile).Name);
                            try
                            {
                                var t = await source.DownloadTextAsync();
                                var newQr = JsonSerializer.Deserialize<QueryRequest>(t);
                                newQr.Visible = false;
                                if (!State.QueryRequests.Exists(qr => qr.Id.CompareTo(newQr.Id) == 0))
                                {
                                    State.QueryRequests.Add(newQr);
                                }
                                else
                                {
                                    var qr = State.QueryRequests.Find(r => r.Id.CompareTo(newQr.Id) == 0);
                                    qr.Visible = false;
                                    qr.Title = newQr.Title;
                                    qr.Query = newQr.Query;
                                }
                            }
                            catch { }
                        }
                        break;
                    case StorageType.Responses:
                        var responses = CloudStorage.ResponsesDirectory;
                        foreach (var f in responses.ListFilesAndDirectories().OfType<CloudFile>())
                        {
                            var source = responses.GetFileReference((f as CloudFile).Name);
                            try
                            {
                                var t = await source.DownloadTextAsync();
                                var newRsp = JsonSerializer.Deserialize<QueryResponse>(t);
                                if (!State.QueryResponses.Exists(rsp => rsp.Id.CompareTo(newRsp.Id) == 0))
                                {
                                    newRsp.Busy = false;
                                    State.QueryResponses.Add(newRsp);
                                }
                            }
                            catch { }
                        }
                        break;
                }

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public RefreshCloudStorageHandler(IStore store, CloudStorageService cloudStorage) : base(store)
            {
                CloudStorage = cloudStorage;
            }
        }
        public class SetE1ContextHander : ActionHandler<SetE1ContextAction>
        {
            AppState State => Store.GetState<AppState>();
            E1Service E1 { get; }
            public override Task<Unit> Handle(SetE1ContextAction aAction, CancellationToken aCancellationToken)
            {
                State.E1Context = State.E1Contexts.Find(r => r.Name.CompareTo(aAction.Name) == 0);
                E1.BaseUrl = State.E1Context.BaseUrl;
                E1.AuthResponse = State.E1Context.AuthResponse;

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public SetE1ContextHander(IStore store, E1Service e1) : base(store)
            {
                E1 = e1;
            }
        }
        public class LoginHandler : ActionHandler<LoginAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(LoginAction aAction, CancellationToken aCancellationToken)
            {
                State.E1Context = aAction.E1Context;
                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);
                return Unit.Task;
            }
            public LoginHandler(IStore store) : base(store) { }
        }
        public class LogoutHandler : ActionHandler<LogoutAction>
        {
            AppState State => Store.GetState<AppState>();
            E1Service E1Service { get; }
            public override async Task<Unit> Handle(LogoutAction aAction, CancellationToken aCancellationToken)
            {
                await E1Service.Logout();
                var ct = State.E1Contexts.Find(r => r.Name.CompareTo(State.E1Context.Name) == 0);
                ct.AuthResponse = null;
                State.E1Context.AuthResponse = null;
                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);
                return Unit.Value;
            }
            public LogoutHandler(IStore store, E1Service e1Service) : base(store)
            {
                E1Service = e1Service;
            }
        }
    }
}
