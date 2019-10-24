using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public class JsService
    {
        IJSRuntime JsRuntime { get; }
        DotNetObjectReference<JsService> ObjectReference { get; }
        public void InitClock(string tag)
        {
            JsRuntime.InvokeVoidAsync("window.clock.init", tag);
        }
        public JsService(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
            ObjectReference = DotNetObjectReference.Create(this);
        }
    }
}
