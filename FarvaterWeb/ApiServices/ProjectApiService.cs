using FarvaterWeb.Configuration;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarvaterWeb.Data;
using Serilog;
using FarvaterWeb.Base;

namespace FarvaterWeb.ApiServices
{
    /*public class ProjectApiService : BaseApiService
    {
        public ProjectApiService(IAPIRequestContext request) : base(request) { }

        public async Task<IAPIRequest> CreateProjectAsync (ProjectModel data)
        {
            if(string.IsNullOrEmpty(_accessToken)) await LoginAsync();

            var url = $"{ConfigurationReader.ApiBaseUrl}api/farvater/data/v1/projects";

            return await _request.PostAsync(url, new()
            {
                DataObject = data,
                Headers = new Dictionary<string, string>
                {
                    {"Authorization", $"Bearer {_accessToken}" },
                    { "Accept", "application/json" }
                }
            });
        }

        public async Task<string?> PrepareProjectAsync() { }

    }*/


}
