using FarvaterWeb.Configuration;
using FarvaterWeb.Data;
using FarvaterWeb.Services;
using Microsoft.Playwright;
using System.Text.Json;

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

            // Используем переменную ApiBaseUrl
            var url = $"{ConfigurationReader.ApiBaseUrl}token?authType=TDMS";

            // 2. Передаем его в параметр Form
            var response = await _request.PostAsync("https://farvater.mcad.dev/token?authType=TDMS", new()
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
            // 1. Проверяем авторизацию
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();

            // 2. ВОТ ЗДЕСЬ задается URL. 
            // Если BaseURL в конфиге уже содержит 'https://farvater.mcad.dev/', 
            // то пишем относительный путь. Если нет — пишем полный.
            return await _request.PostAsync("https://farvater.mcad.dev/api/farvater/data/v1/contractors/legal", new()
            {
                DataObject = data,
                Headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {_accessToken}" },
            { "Accept", "application/json" }
        }
            });
        }
    }
}