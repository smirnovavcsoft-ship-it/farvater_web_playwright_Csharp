using FarvaterWeb.Configuration;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.ApiServices
{
    public class UserApiService : BaseApiService
    {
        public UserApiService(IAPIRequestContext request) : base(request) { }

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


    }
}
