using System;

namespace DataBrowser.Features.AppState
{
    public class SaveQueryRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Query { get; set; } = string.Empty;
    }
    public class QueryRequest : SaveQueryRequest
    {
        public bool Visible { get; set; }
        public DateTime Toggled { get; set; }
    }
}
