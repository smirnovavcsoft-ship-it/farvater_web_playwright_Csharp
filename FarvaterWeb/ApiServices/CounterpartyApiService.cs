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

       

        public async Task<string?> PrepareCounterpartyAsync(string title, string shortTitle, string inn)
        {
            string? createdHandle = null;

            await HelperForReports.Do("API: Создание контрагента", async () =>
            {
                var model = new CounterpartyModel
                {
                    title = title,
                    shorttitle = shortTitle,
                    inn = inn
                };

                var response = await CreateCounterpartyAsync(model);

                if (response.Ok)
                {
                    var json = await response.JsonAsync();
                    // Вытаскиваем именно handle
                    createdHandle = json?.GetProperty("handle").GetString();
                    Log.Information($"[API] Контрагент создан. Handle: {createdHandle}");
                }
                else if (response.Status != 400 && response.Status != 409)
                {
                    var details = await response.TextAsync();
                    throw new Exception($"Ошибка API ({response.Status}): {details}");
                }

                Log.Information($"[API] Подготовка завершена (Status: {response.Status})");
            });

            return createdHandle; // Теперь здесь будет реальный TH..., а не null
        }

        public async Task<IAPIResponse> DeleteCounterpartyAsync(string? handle)
        {
            if (string.IsNullOrEmpty(handle))
            {
                Log.Warning("[API] Попытка удаления: Handle пустой.");
                return null!;
            }

            var cleanHandle = handle.Trim('{', '}');
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();

            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/contractors/{cleanHandle}";

            // Отправляем запрос и сохраняем ответ в переменную
            var response = await _request!.DeleteAsync(url, new()
            {
                Headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {_accessToken}" },
            { "Accept", "application/json" }
        }
            });

            // ВАЖНО: Добавляем вывод статуса в логи
            Log.Information($"[API] Результат удаления {cleanHandle}: {(int)response.Status} {response.StatusText}");

            // Если сервер прислал ошибку, полезно увидеть текст
            if (!response.Ok)
            {
                var errorBody = await response.TextAsync();
                Log.Error($"[API] Детали ошибки удаления: {errorBody}");
            }

            return response;
        }
    }
}