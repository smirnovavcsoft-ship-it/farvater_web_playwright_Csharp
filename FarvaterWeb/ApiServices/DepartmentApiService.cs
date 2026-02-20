using FarvaterWeb.Configuration;
using Microsoft.Playwright;
using FarvaterWeb.Data;
using Serilog;
using FarvaterWeb.Base;

namespace FarvaterWeb.ApiServices
{
    public class DepartmentApiService : BaseApiService
    {
        public DepartmentApiService(IAPIRequestContext request) : base(request) { }



        public async Task<IAPIResponse> CreateDepartmentAsync(DepartmentModel data)
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();
            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/departments";
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

        public async Task<string?> PrepareDepartmentAsync(string description, string code)
        {
            string? createdHandle = null;
            await HelperForReports.Do("API: Создание отдела", async () =>
            {
                var departmentData = new DepartmentModel
                {
                    Name = description,
                    Code = code
                };
                var response = await CreateDepartmentAsync(departmentData);
                if (response.Ok)
                {
                    var json = await response.JsonAsync();
                    createdHandle = json?.GetProperty("handle").GetString();
                }
            });
            return createdHandle;
        }

        public async Task<IAPIResponse> DeleteDepartmentAsync(string handle)
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();
            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/departments/{handle}";
            return await _request.DeleteAsync(url, new()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {_accessToken}" },
                    { "Accept", "application/json" }
                }
            });
        }
    }
}
