using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public class E1Service : Celin.AIS.Server
    {
        public static readonly string SESSIONKEY = "E1";
        SessionStorage SessionStorage { get; }
        public async Task<Celin.AIS.AuthResponse> Login(string user, string pwd)
        {
            try
            {
                AuthRequest.username = user;
                AuthRequest.password = pwd;
                await AuthenticateAsync();
                await SessionStorage.SetItemAsync(SESSIONKEY, AuthResponse);
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
            await SessionStorage.RemoveItemAsync(SESSIONKEY);
        }
        public E1Service(IConfiguration configuration, ILogger<E1Service> logger, IHttpClientFactory httpClientFactory, SessionStorage sessionStorage)
            : base(configuration["baseUrl"], logger, httpClientFactory.CreateClient())
        {
            SessionStorage = sessionStorage;
        }
    }
}
