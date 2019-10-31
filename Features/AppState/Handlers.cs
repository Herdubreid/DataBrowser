using BlazorState;
using DataBrowser.Services;
using MediatR;
using Pidgin;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unit = MediatR.Unit;

namespace DataBrowser.Features.AppState
{
    public partial class AppState
    {
        public class NotifyChangeHandler : ActionHandler<NotifyChangeAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(NotifyChangeAction aAction, CancellationToken aCancellationToken)
            {
                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public NotifyChangeHandler(IStore store) : base(store) { }
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
                    Name = $"New-{State.QueryRequests.Count}"
                };
                State.QueryRequests.Insert(0, qr);

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

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public ToggleQueryRequestVisibilityHandler(IStore store) : base(store) { }
        }
        public class GetJsonHandler : ActionHandler<GetJsonAction>
        {
            AppState State => Store.GetState<AppState>();
            JsService Js { get; }
            public override async Task<Unit> Handle(GetJsonAction aAction, CancellationToken aCancellationToken)
            {
                var query = await Js.GetEditorTextAsync(aAction.Source);
                if (query.Length > 0)
                {
                    try
                    {
                        query += ';';
                        var result = Celin.AIS.Data.DataRequest.Parser.Before(Parser.Char(';')).ParseOrThrow(query);
                        Js.SetJsonText(aAction.Destination, JsonSerializer.Serialize(result, new JsonSerializerOptions
                        {
                            IgnoreNullValues = true,
                            WriteIndented = true
                        }));
                    }
                    catch (ParseException e)
                    {
                        Js.SetJsonText(aAction.Destination, e.Message);
                    }
                }
                return Unit.Value;
            }
            public GetJsonHandler(IStore store, JsService jsService) : base(store)
            {
                Js = jsService;
            }
        }
        public class LoginHandler : ActionHandler<LoginAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(LoginAction aAction, CancellationToken aCancellationToken)
            {
                State.AuthResponse = aAction.AuthResponse;
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
                State.AuthResponse = null;
                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);
                return Unit.Value;
            }
            public LogoutHandler(IStore store, E1Service e1Service) : base(store)
            {
                E1Service = e1Service;
                E1Service.AuthResponse = State.AuthResponse;
            }
        }
    }
}
