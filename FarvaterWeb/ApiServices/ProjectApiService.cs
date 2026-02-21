using FarvaterWeb.Base;
using FarvaterWeb.Configuration;
using FarvaterWeb.Data;
using FarvaterWeb.Generators;
using Microsoft.Playwright;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FarvaterWeb.ApiServices
{
    public class ProjectApiService : BaseApiService
    {
        private readonly UserApiService _userApiService;

        // В конструктор теперь передаем и request, и сервис пользователей
        public ProjectApiService(IAPIRequestContext request, UserApiService userApiService) : base(request)
        {
            _userApiService = userApiService;
        }

        // --- НИЗКОУРОВНЕВЫЕ МЕТОДЫ (Твои кирпичики) ---

        public async Task<string?> GetCapitalConstructionHandleAsync(string title)
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();
            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/references/capital-construction-types";
            var response = await _request.GetAsync(url, GetAuthHeaders());

            var json = await response.JsonAsync();
            return json?.GetProperty("items").EnumerateArray()
                        .FirstOrDefault(i => i.GetProperty("title").GetString() == title)
                        .GetProperty("handle").GetString();
        }

        public async Task<string?> GetProjectTypeHandleAsync(string title)
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();
            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/references/project-types";
            var response = await _request.GetAsync(url, GetAuthHeaders());

            var json = await response.JsonAsync();
            return json?.GetProperty("items").EnumerateArray()
                        .FirstOrDefault(i => i.GetProperty("title").GetString() == title)
                        .GetProperty("handle").GetString();
        }

        public async Task<IAPIResponse> CreateProjectRequestAsync(ProjectModel data)
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();
            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/projects";

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

        // --- ВЫСОКОУРОВНЕВЫЙ МЕТОД (Та самая "Одна строка") ---

        public async Task<string?> PrepareAndCreateProjectAsync(string constrTitle, string typeTitle, string lastName, string firstName, string login)
        {
            Log.Information($"[API] Начинаем комплексное создание проекта: {constrTitle} / {typeTitle}");

            // 1. Создаем ГИПа, используя ВТОРОЙ сервис
           // var gipLogin = $"gip_{Guid.NewGuid().ToString()[..4]}";
            var gipHandle = await _userApiService.PrepareUserAsync(lastName, firstName, login);

            // 2. Получаем хэндлы справочников
            var constrHandle = await GetCapitalConstructionHandleAsync(constrTitle);
            var projTypeHandle = await GetProjectTypeHandleAsync(typeTitle);

            // 3. Собираем модель
            var model = DataFactory.GenerateProjectModel();
            model.Gip.Handle = gipHandle;
            model.CapitalConstructionType.Handle = constrHandle;
            model.ProjectType.Handle = projTypeHandle;

            // 4. Создаем проект
            var response = await CreateProjectRequestAsync(model);

            if (response.Ok)
            {
                var json = await response.JsonAsync();
                return json?.GetProperty("handle").GetString();
            }

            Log.Error($"[API] Не удалось создать проект: {await response.TextAsync()}");
            return null;
        }

        private APIRequestContextOptions GetAuthHeaders() => new()
        {
            Headers = new Dictionary<string, string> { { "Authorization", $"Bearer {_accessToken}" } }
        };
    }
}
