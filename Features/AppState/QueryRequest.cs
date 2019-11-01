using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataBrowser.Features.AppState
{
    public class QueryRequest
    {
        [JsonIgnore]
        public bool Visible { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; } = string.Empty;
    }
}
