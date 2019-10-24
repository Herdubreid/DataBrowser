using BlazorState;
using MyApp.Features.AppState;
using System;

namespace MyApp.Pages
{
    public class E1BaseComponent : BlazorStateComponent
    {
        protected AppState State => Store.GetState<AppState>();
        protected void Update(object sender, EventArgs args) => StateHasChanged();
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
