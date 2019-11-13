using BlazorState;
using DataBrowser.Features.AppState;
using System;

namespace DataBrowser.Pages
{
    public class E1BaseComponent : BlazorStateComponent
    {
        protected AppState State => Store.GetState<AppState>();
        protected void Update(object sender, EventArgs args) => InvokeAsync(StateHasChanged);
        protected override void OnInitialized()
        {
            State.Changed += Update;
        }
        public new void Dispose()
        {
            State.Changed -= Update;
            base.Dispose();
        }
    }
}
