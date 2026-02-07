using FarvaterWeb.Base;
using FarvaterWeb.Configuration;
using FarvaterWeb.Data;
using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.ApiServices
{
    public class CounterpartyApiService : BaseApiService
    {
        
        // protected string? _accessToken;

        // Поле _api удалено, так как оно вызывало NullReferenceException

        public CounterpartyApiService(IAPIRequestContext request) : base(request) { }
        

        

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