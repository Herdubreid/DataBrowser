using DataBrowser.Services;

namespace DataBrowser.Features.AppState
{
    public class SaveE1Context
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string User { get; set; }
    }
    public class E1Context : SaveE1Context
    {
        public Celin.AIS.AuthResponse AuthResponse { get; set; }
    }
}
