using Blazored.LocalStorage;
using CRM.BLL.DTO;
using CRM.BLL.Interfaces;
using CRM.DAL.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CRM.BLAZOR.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly ITempService _tempService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly UserManager<User> _userManager;
        private readonly ILogService _logService; 
        private static IHttpContextAccessor _httpContextAccessor;
        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage, ITempService tempService, 
                           IUserRegistrationService userRegistrationService, UserManager<User> userManager,
                           ILogService logService, HttpClientHandler httpClientHandler,
                           IHttpContextAccessor httpContextAccessor)
        {
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient(httpClientHandler);
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _tempService = tempService;
            _userRegistrationService = userRegistrationService;
            _userManager = userManager;
            _logService = logService;
        }
        private static Uri GetAbsoluteUri()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if(request.Host.Port!=null)
            {
                uriBuilder.Port = request.Host.Port.Value;
            }
            //uriBuilder.Path = request.Path.ToString();
            //uriBuilder.Query = request.QueryString.ToString();
            return uriBuilder.Uri;
        }
        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var url = GetAbsoluteUri().OriginalString;
            var response = await _httpClient.PostAsync(url+"Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }

            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            _tempService.CurrentUser = await _userRegistrationService.GetCurrent(loginModel.Email);
            User user = await _userManager.FindByEmailAsync(loginModel.Email);
            LogDTO logDTO = new LogDTO
            {
                Action = "Успешно авторизовался",
                UserId = user.Id
            };
            await _logService.AddLog(logDTO);

            return loginResult;
        }

        public async Task Logout()
        {
            var userId = _tempService.CurrentUser.Id;
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            LogDTO logDTO = new LogDTO
            {
                Action = "Вышел из системы",
                UserId = userId
            };
            await _logService.AddLog(logDTO);
            _tempService.CurrentUser = null;
        }
    }
}
