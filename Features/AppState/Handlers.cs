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
                        query = Regex.Replace(query, @"\t|\n|\r", string.Empty).Trim(' ') + '\n';
                        var result = Celin.AIS.Data.DataRequest.Parser.AtLeastOnceUntil(Parser.EndOfLine).ParseOrThrow(query);
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
