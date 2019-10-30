using BlazorState;
using DataBrowser.Services;
using MediatR;
using Pidgin;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Unit = MediatR.Unit;

namespace DataBrowser.Features.AppState
{
    public partial class AppState
    {
        public class GetJsonHandler : ActionHandler<GetJsonAction>
        {
            AppState State => Store.GetState<AppState>();
            JsService Js { get; }
            public override Task<Unit> Handle(GetJsonAction aAction, CancellationToken aCancellationToken)
            {
                if (aAction.Command.Length > 0)
                {
                    try
                    {
                        var result = Celin.AIS.Data.DataRequest.Parser.Parse(aAction.Command);
                        Js.SetJsonText(0, JsonSerializer.Serialize(result.Value, new JsonSerializerOptions
                        {
                            IgnoreNullValues = true,
                            WriteIndented = true
                        }));
                    }
                    catch (Exception e)
                    { }
                }
                return Unit.Task;
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
                State.LoginTime = DateTime.Now;
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
