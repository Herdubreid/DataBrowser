using BlazorState;
using System;
using System.Collections.Generic;

namespace DataBrowser.Features.AppState
{
    public partial class AppState : State<AppState>
    {
        public event EventHandler Changed;
        public bool InRequest { get; set; }
        public List<Guid> ResponseData { get; set; }
        public List<Guid> DemoData { get; set; }
        public List<QueryResponse> QueryResponses { get; set; }
        public List<QueryRequest> QueryRequests { get; set; }
        public List<E1Context> E1Contexts { get; set; }
        public E1Context E1Context { get; set; }
        public bool Authenticated => E1Context?.AuthResponse != null;
        public override void Initialize()
        {
            ResponseData = new List<Guid>();
            DemoData = new List<Guid>();
            QueryResponses = new List<QueryResponse>();
            QueryRequests = new List<QueryRequest>();
            E1Contexts = new List<E1Context>();
        }
    }
}
