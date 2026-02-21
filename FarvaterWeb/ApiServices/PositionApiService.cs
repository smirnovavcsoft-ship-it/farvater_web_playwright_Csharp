using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarvaterWeb.Configuration;
using FarvaterWeb.Data;
using Serilog;
using FarvaterWeb.Base;

namespace FarvaterWeb.ApiServices
{
    public class PositionApiService : BaseApiService
    {
        public PositionApiService(IAPIRequestContext request) : base(request) { }

        public async Task<IAPIResponse> CreatePositionAsync(PositionModel data)
        {
            if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();
            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/positions";
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

        public async Task<string?> PreparePositionAsync(string description)
        {
            string? createdHandle = null;
            await HelperForReports.Do("API: Создание должности", async () =>
            {
                var positionData = new PositionModel
                {
                    Description = description,                    
                };
                var response = await CreatePositionAsync(positionData);
                if (response.Ok)
                {
                    var json = await response.JsonAsync();
                    createdHandle = json?.GetProperty("handle").GetString();
                }
            });
            return createdHandle;
        }

            public async Task<IAPIResponse> DeletePositionAsync(string handle)
            {
                if (string.IsNullOrEmpty(_accessToken)) await LoginAsync();
                var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/positions/{handle}";
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
