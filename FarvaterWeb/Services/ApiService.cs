using Microsoft.Playwright;
using System.Text.Json;
using FarvaterWeb.Services;
using FarvaterWeb.Data;

namespace FarvaterWeb.Services
{
    public class ApiService
    {
        private readonly IAPIRequestContext _request;
        private string? _accessToken;

        public ApiService(IAPIRequestContext request)
        {
            _request = request;
        }

        public async Task LoginAsync()
        {
            // 1. Создаем специальный объект FormData
            var formData = _request.CreateFormData();
            formData.Append("username", "SYSADMIN");
            formData.Append("password", "");
            formData.Append("grant_type", "password");
            formData.Append("client_id", "Web");
            formData.Append("authType", "TDMS");

            // 2. Передаем его в параметр Form
            var response = await _request.PostAsync("/token?authType=TDMS", new()
            {
                Form = formData, // Теперь типы совпадают!
                Headers = new Dictionary<string, string>
        {
            { "Accept", "application/json; charset=utf-8" }
        }
            });

            if (!response.Ok)
                throw new Exception($"Login failed: {response.StatusText}");

            var json = await response.JsonAsync();
            // Используем GetProperty и GetString для извлечения токена
            _accessToken = json?.GetProperty("access_token").GetString();
        }

        public async Task<IAPIResponse> CreateCounterpartyAsync(CounterpartyModel data)
        {
            // Если токена нет, сначала логинимся
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();

            return await _request.PostAsync("api/v1/counterparties", new()
            {
                DataObject = data,
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {_accessToken}" }
                }
            });
        }
    }
}