using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace DataBrowser.Services
{
    public class JsService
    {
        IJSRuntime JsRuntime { get; }
        DotNetObjectReference<JsService> ObjectReference { get; }
        public void InitEditor(string id, string text)
        {
            JsRuntime.InvokeVoidAsync("window.cqlEditor.init", id, text);
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
        public JsService(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
            ObjectReference = DotNetObjectReference.Create(this);
        }
    }
}
