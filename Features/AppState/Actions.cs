using BlazorState;

namespace DataBrowser.Features.AppState
{
    public class GetJsonAction : IAction
    {
        public string Command { get; set; }
    }
    public class LoginAction : IAction
    {
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
    }
    public class LogoutAction : IAction { }
}
