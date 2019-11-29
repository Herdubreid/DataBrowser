using System;
using System.Collections.Generic;

namespace DataBrowser.Features
{
    public class QueryResponseData<T>
    {
        public IEnumerable<T> rowset { get; set; }
        public IEnumerable<T> output { get; set; }
    }
    public class SaveQueryResponse
    {
        public Guid Id { get; set; }
        public string Query { get; set; }
        public string Title { get; set; }
        public List<string> Columns { get; set; }
        public string Environment { get; set; }
        public DateTime Submitted { get; set; }
        public Celin.AIS.Summary Summary { get; set; }
        public int Count { get; set; }
        public bool Demo { get; set; }
    }
    public class QueryResponse : SaveQueryResponse
    {
        public string Error { get; set; }
        public bool Busy { get; set; }
    }
}
