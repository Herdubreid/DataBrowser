using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.Services
{
    public class JsService
    {
        IJSRuntime JsRuntime { get; }
        DotNetObjectReference<JsService> ObjectReference { get; }
        public async Task<int> InitEditorAsync(string id, string text)
        {
            return await JsRuntime.InvokeAsync<int>("window.cqlEditor.init", id, text);
        }
        public async Task<string> GetEditorTextAsync(int index)
        {
            return await JsRuntime.InvokeAsync<string>("window.cqlEditor.getText", index);
        }
        public async Task<int> InitJsonViewerAsync(string id, string text)
        {
            return await JsRuntime.InvokeAsync<int>("window.jsonViewer.init", id, text);
        }
        public void SetJsonText(int index, string text)
        {
            JsRuntime.InvokeVoidAsync("window.jsonViewer.setText", index, text);
        }
        public JsService(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
            ObjectReference = DotNetObjectReference.Create(this);
        }
    }
}
