using BlazorState;
using DataBrowser.Services;
using Microsoft.Azure.Storage.File;
using OfficeOpenXml;
using Pidgin;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
                    var rows = data.output ?? data.rowset;
                    using var xl = new ExcelPackage();
                    var ws = xl.Workbook.Worksheets.Add("Data");
                    var header = new List<string[]>
                    {
                        new string[] { "Title:", rsp.Title },
                        new string[] { "Query:", rsp.Query },
                        new string[] { "Environment:", rsp.Environment },
                        new string[] { "Submitted:", rsp.Submitted.ToString() },
                        rsp.Columns.Select(c => c.Key).ToArray()
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
                    case ResponseAction.VIEW:
                        State.ResponseData.Insert(0, aAction.DataId);
                        break;
                    case ResponseAction.REMOVE:
                        State.ResponseData.Remove(aAction.DataId);
                        break;
                    case ResponseAction.CLEAR:
                        State.QueryResponses.Remove(State.QueryResponses.Find(r => r.Id.CompareTo(aAction.DataId) == 0));
                        break;
                    case ResponseAction.DELETE:
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
                    try
                    {
                        var request = Celin.AIS.Data.DataRequest.Parser.Before(Parser.Char(';')).ParseOrThrow(qr.Query + ';');
                        var rsp = new QueryResponse { Id = Guid.NewGuid(), Query = qr.Query };
                        State.QueryResponses.Insert(0, rsp);
                        State.InRequest = true;
                        await rsp.Submit(E1, rsp.Id, request, handler, CloudStorage.ResponsesDirectory);
                    }
                    catch (ParseException e)
                    {
                        Js.SetJsonText(qr.Id.ToString(), e.Message);
                    }
                    finally
                    {
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
                        var result = Celin.AIS.Data.DataRequest.Parser.Before(Parser.Char(';')).ParseOrThrow(qr.Query + ';');
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
