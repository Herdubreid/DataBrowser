using BlazorState;

namespace DataBrowser.Features.AppState
{
    public class GetJsonAction : IAction
    {
        public int Source { get; set; }
        public int Destination { get; set; }
    }
    public class LoginAction : IAction
    {
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
    }
    public class LogoutAction : IAction { }
}
