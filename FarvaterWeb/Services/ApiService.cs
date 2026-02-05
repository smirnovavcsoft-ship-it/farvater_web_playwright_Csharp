using FarvaterWeb.Base;
using FarvaterWeb.Configuration;
using FarvaterWeb.Data;
using FarvaterWeb.Services;
using Microsoft.Playwright;
using System.Text.Json;
using Serilog;

namespace FarvaterWeb.Services
{
    public class ApiService
    {
        private readonly IAPIRequestContext _request;
        private string? _accessToken;

        // Поле _api удалено, так как оно вызывало NullReferenceException

        public ApiService(IAPIRequestContext request)
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

        public async Task<IAPIResponse> CreateCounterpartyAsync(CounterpartyModel data)
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();

            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/contractors/legal";

            return await _request.PostAsync(url, new()
            {
                DataObject = data,
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {_accessToken}" },
                    { "Accept", "application/json" }
                }
            });
        }

        /*public async Task PrepareCounterpartyAsync(string title, string shortTitle, string inn)
        {
            var model = new CounterpartyModel
            {
                inn = inn,
                title = title,
                shorttitle = shortTitle,
                address = "",
                ogrn = "",
                kpp = "",
                phone = "",
                email = ""
            };

            // ВЫЗЫВАЕМ МЕТОД ЭТОГО ЖЕ КЛАССА (вместо _api. ...)
            var response = await CreateCounterpartyAsync(model);

            if (!response.Ok && response.Status != 400 && response.Status != 409)
            {
                var details = await response.TextAsync();
                throw new Exception($"Некорректный ответ сервера API ({response.Status}): {details}");
            }
        }*/

        public async Task PrepareCounterpartyAsync(string title, string shortTitle, string inn)
        {
            // Используем Page.Do, чтобы это попало в логи и Allure
            // Внимание: для этого у ApiService должен быть доступ к _page
            await HelperForReports.Do("API: Создание контрагента", async () =>
            {
                var model = new CounterpartyModel
                {
                    title = title,
                    shorttitle = shortTitle,
                    inn = inn
                };

                var response = await CreateCounterpartyAsync(model);

                if (!response.Ok && response.Status != 400 && response.Status != 409)
                {
                    var details = await response.TextAsync();
                    throw new Exception($"Ошибка API ({response.Status}): {details}");
                }

                Log.Information($"[API] Контрагент готов (Status: {response.Status})");
            });
        }
    }
}