using BlazorState;
using System;
using DataBrowser.Services;

namespace DataBrowser.Features.AppState
{
    public enum ResponseAction
    {
        VIEW,
        REMOVE,
        CLEAR,
        DELETE
    }
    public partial class AppState
    {
        public class SetE1ContextAction : IAction
        {
            public string Name { get; set; }
        }
        public class ResponseDataDownloadAction : IAction
        {
            public Guid DataId { get; set; }
        }
        public class ResponseDataAction : IAction
        {
            public ResponseAction Action { get; set; }
            public Guid DataId { get; set; }
        }
        public class AddE1ContextAction : IAction
        {
            public string Name { get; set; }
            public string BaseUrl { get; set; }
        }
        public class SaveQueryRequestAction : IAction
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Query { get; set; }
        }
        public class DeleteQueryRequestAction : IAction
        {
            public Guid Id { get; set; }
        }
        public class AddNewQueryRequestAction : IAction
        {
            public string Title { get; set; }
            public string Query { get; set; }
        }
        public class ToggleQueryRequestVisibilityAction : IAction
        {
            public Guid Id { get; set; }
        }
        public class ValidateRequestAction : IAction
        {
            public Guid Id { get; set; }
        }
        public class SubmitQueryAction : IAction
        {
            public Guid Id { get; set; }
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
