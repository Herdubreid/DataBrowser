using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.Services
{
    public class E1Service : Celin.AIS.Server
    {
        public IDictionary<Guid, CancellationTokenSource> CancelTokens { get; }
            = new Dictionary<Guid, CancellationTokenSource>();
        public void CancelRequest(Guid id)
        {
            if (CancelTokens.TryGetValue(id, out CancellationTokenSource cancel))
            {
                cancel.Cancel();
            }
        }
        public async Task<Celin.AIS.AuthResponse> Login(string user, string pwd)
        {
            try
            {
                AuthRequest.username = user;
                AuthRequest.password = pwd;
                await AuthenticateAsync();
                return AuthResponse;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task Logout()
        {
            await LogoutAsync();
        }
        public E1Service(ILogger<E1Service> logger, IHttpClientFactory httpClientFactory)
            : base("", logger, httpClientFactory.CreateClient()) { }
    }
}
