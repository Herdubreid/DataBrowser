using BlazorState;
using System;

namespace DataBrowser.Features.AppState
{
    public partial class AppState : State<AppState>
    {
        public event EventHandler Changed;
        public string Json { get; set; }
        public string ErrorMsg { get; set; }
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
        public bool Authenticated => AuthResponse != null;
        public DateTime LoginTime { get; set; }
        public override void Initialize() { }
    }
}
