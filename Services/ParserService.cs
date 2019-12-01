using Pidgin;
using System.Text.Json;

namespace DataBrowser.Services
{
    public class ParserService
    {
        public Celin.AIS.DatabrowserRequest Parse(string qry) =>
            Celin.AIS.Data.DataRequest.Parser.Before(Parser.Char(';')).ParseOrThrow(qry.TrimEnd() + ';');
        public string ToString(string qry)
        {
            try
            {
                var request = Parse(qry);
                return JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = true
                });
            }
            catch (ParseException e)
            {
                return e.Message;
            }
        }
    }
}
