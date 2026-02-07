using FarvaterWeb.Base;
using FarvaterWeb.Configuration;
using FarvaterWeb.Data;
using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.ApiServices
{
    public class BaseApiService
    {
        protected readonly IAPIRequestContext? _request;
        protected string? _accessToken;

        // Поле _api удалено, так как оно вызывало NullReferenceException

        public BaseApiService(IAPIRequestContext? request = null)
        {
            _request = request;
        }

        public async Task LoginAsync()
        {
            var formData = _request.CreateFormData();
            formData.Append("username", "SYSADMIN");
            formData.Append("password", "");
            formData.Append("grant_type", "password");
            formData.Append("client_id", "Web");
            formData.Append("authType", "TDMS");

            var url = $"{ConfigurationReader.ApiBaseUrl}token?authType=TDMS";

            var response = await _request.PostAsync(url, new()
            {
                Form = formData,
                Headers = new Dictionary<string, string>
                {
                    { "Accept", "application/json; charset=utf-8" }
                }
            });

            if (!response.Ok)
            {
                var errorText = await response.TextAsync();
                throw new Exception($"Login failed: {response.Status} {errorText}");
            }

            var json = await response.JsonAsync();
            _accessToken = json?.GetProperty("access_token").GetString();
        }

    }
}