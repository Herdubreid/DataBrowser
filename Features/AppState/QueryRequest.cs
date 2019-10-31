using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.Features.AppState
{
    public class QueryRequest
    {
        public bool Visible { get; set; }
        public Guid Id { get; } = new Guid();
        public string Name { get; set; }
        public string Query { get; set; } = string.Empty;
    }
}
