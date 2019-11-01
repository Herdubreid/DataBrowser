using BlazorState;
using System;

namespace DataBrowser.Features.AppState
{
    public class NotifyChangeAction : IAction { }
    public class SaveQueryRequestAction : IAction
    {
        public Guid Id { get; set; }
    }
    public class DeleteQueryRequestAction : IAction
    {
        public Guid Id { get; set; }
    }
    public class AddNewQueryRequestAction : IAction { }
    public class ToggleQueryRequestVisibilityAction : IAction
    {
        public Guid Id { get; set; }
    }
    public class GetJsonAction : IAction
    {
        public Guid Id { get; set; }
        public int Source { get; set; }
        public int Destination { get; set; }
    }
    public class LoginAction : IAction
    {
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
    }
    public class LogoutAction : IAction { }
}
