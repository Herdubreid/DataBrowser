using BlazorState;
using System;

namespace MyApp.Features.AppState
{
    public partial class AppState : State<AppState>
    {
        public event EventHandler Changed;
        public string ErrorMsg { get; set; }
        public bool EditMode { get; set; }
        public E1.F0101.Row[] AddressBookList { get; set; }
        public E1.W01012A.Form AddressBook { get; set; }
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
        public bool Authenticated => AuthResponse != null;
        public DateTime LoginTime { get; set; }
        public override void Initialize() { }
    }
}
