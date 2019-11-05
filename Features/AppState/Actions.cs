using BlazorState;
using System;
using DataBrowser.Services;

namespace DataBrowser.Features.AppState
{
    public partial class AppState
    {
        public class AddE1ContextAction : IAction
        {
            public string Name { get; set; }
            public string BaseUrl { get; set; }
        }
        public class SaveQueryRequestAction : IAction
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
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
        public class RefreshCloudStorageAction : IAction
        {
            public StorageType StorageType { get; set; }
        } 
        public class LoginAction : IAction
        {
            public E1Context E1Context { get; set; }
        }
        public class LogoutAction : IAction { }
    }
}
