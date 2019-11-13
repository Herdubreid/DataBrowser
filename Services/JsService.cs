using DataBrowser.Features.AppState;
using MediatR;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace DataBrowser.Services
{
    public class JsService
    {
        IJSRuntime JsRuntime { get; }
        IMediator Mediator { get; }
        DotNetObjectReference<JsService> Ref { get; }
        [JSInvokable]
        public void TextChanged(string id, string qry)
        {
            var guid = new Guid(id);
            Mediator.Send(new AppState.SaveQueryRequestAction { Id = guid, Query = qry });
        }
        public void SaveAs(string filename, byte[] data)
        {
            JsRuntime.InvokeVoidAsync("window.saveAsFile", filename, Convert.ToBase64String(data));
        }
        public void InitEditor(string id, string text)
        {
            JsRuntime.InvokeVoidAsync("window.cqlEditor.init", Ref, id, text);
        }
        public async Task<string> GetEditorTextAsync(string id)
        {
            return await JsRuntime.InvokeAsync<string>("window.cqlEditor.getText", id);
        }
        public void InitJsonViewer(string id, string text)
        {
            JsRuntime.InvokeVoidAsync("window.jsonViewer.init", id, text);
        }
        public void SetJsonText(string id, string text)
        {
            JsRuntime.InvokeVoidAsync("window.jsonViewer.setText", id, text);
        }
        public JsService(IJSRuntime jsRuntime, IMediator mediator)
        {
            JsRuntime = jsRuntime;
            Mediator = mediator;
            Ref = DotNetObjectReference.Create(this);
        }
    }
}
