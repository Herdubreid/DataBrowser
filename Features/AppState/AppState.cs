using BlazorState;
using System;
using System.Collections.Generic;

namespace DataBrowser.Features.AppState
{
    public partial class AppState : State<AppState>
    {
        public event EventHandler Changed;
        public List<QueryRequest> QueryRequests { get; } = new List<QueryRequest>();
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
        public bool Authenticated => AuthResponse != null;
        public override void Initialize()
        {
            QueryRequests.Add(new QueryRequest { Name = "New", Visible = true });
        }
    }
}
