using FarvaterWeb.Configuration;
using Microsoft.Playwright;
using FarvaterWeb.Data;
using Serilog;
using FarvaterWeb.Base;

namespace FarvaterWeb.ApiServices
{
    public class UserApiService : BaseApiService
    {
        public UserApiService(IAPIRequestContext request) : base(request) { }

        public async Task<string?> GetCurrentUserInfoAsync()
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();

            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/users/current";

            var response = await _request.GetAsync(url, new()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {_accessToken}" },
                    { "Accept", "application/json" }
                }
            });

            if (!response.Ok) return null;

            var json = await response.JsonAsync();
            // Пытаемся достать handle из ответа
            return json?.GetProperty("handle").GetString();
        }

        public async Task<IAPIResponse> CreateUserAsync(UserModel data)
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();

            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/users";

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




        public async Task<string?> PrepareUserAsync(string lastName, string firstName, string login, bool isDomainUser = false, string language = "ru")
        {
            string? createdHandle = null;

            await HelperForReports.Do("API: Создание пользователя", async () =>
            {
                var model = new UserModel
                {
                    LastName = lastName,
                    FirstName = firstName,
                    Login = login,
                    IsDomainUser = isDomainUser,
                    Language = language
                };

                var response = await CreateUserAsync(model);

                if (response.Ok)
                {
                    var json = await response.JsonAsync();
                    createdHandle = json?.GetProperty("handle").GetString();
                    Log.Information($"[API] Пользователь создан. Handle: {createdHandle}");
                }
                else if (response.Status != 400 && response.Status != 409)
                {
                    var details = await response.TextAsync();
                    throw new Exception($"Ошибка API ({response.Status}): {details}");
                }

                Log.Information($"[API] Подготовка завершена (Status: {response.Status})");
            });

            return createdHandle;
        }

        public async Task<IAPIResponse> DismissUserAsync(string? dismissedHandle)
        {
            if (string.IsNullOrEmpty(dismissedHandle))
            {
                Log.Warning("[API] Попытка увольнения: Handle увольняемого пустой.");
                return null!;
            }

            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();

            // Узнаем, кому передать дела (текущему админу)
            var currentAdminHandle = await GetCurrentUserInfoAsync();

            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/users/dismiss";

            // Формируем объект запроса согласно твоему JSON
            var data = new
            {
                user = new { handle = dismissedHandle },
                substitute = new { handle = currentAdminHandle }
            };

            var response = await _request.PostAsync(url, new()
            {
                DataObject = data,
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {_accessToken}" },
                    { "Accept", "application/json" }
                }
            });

            Log.Information($"[API] Результат увольнения {dismissedHandle}: {(int)response.Status} {response.StatusText}");

            if (!response.Ok)
            {
                var errorBody = await response.TextAsync();
                Log.Error($"[API] Детали ошибки увольнения: {errorBody}");
            }

            return response;
        }
    }

}


    
