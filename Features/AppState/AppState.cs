using BlazorState;
using DataBrowser.Services;
using System;
using System.Collections.Generic;

namespace DataBrowser.Features.AppState
{
    public partial class AppState : State<AppState>
    {
        public event EventHandler Changed;
        public List<QueryRequest> QueryRequests { get; } = new List<QueryRequest>();
        public List<E1Context> E1Contexts { get; } = new List<E1Context>();
        public E1Context E1Context { get; set; }
        public bool Authenticated => E1Context?.AuthResponse != null;
        public override void Initialize() { }
        public AppState() { }
    }
}
