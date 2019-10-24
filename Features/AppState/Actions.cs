using BlazorState;

namespace MyApp.Features.AppState
{
    public enum FormAction
    {
        Get, Save
    }
    public class AddressBookAction : IAction
    {
        public FormAction FormAction { get; set; }
        public string AN8 { get; set; }
    }
    public class EditModeAction : IAction
    {
        public bool EditMode { get; set; }
    }
    public class ListAddressBookAction : IAction
    {
        public string Search { get; set; }
    }
    public class LoginAction : IAction
    {
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
    }
    public class LogoutAction : IAction { }
}
