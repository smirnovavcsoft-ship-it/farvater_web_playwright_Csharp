using Microsoft.Playwright;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public class ApiService
    {
        private readonly IAPIRequestContext _request;

        public ApiService(IAPIRequestContext request)
        {
            _request = request;
        }

        public async Task<IAPIResponse> CreateCounterpartyAsync(CounterpartyModel data)
        {
            // Отправляем POST запрос с нашим объектом
            // Playwright сам конвертирует CounterpartyModel в JSON
            return await _request.PostAsync("путь/к/вашему/api", new APIRequestContextOptions
            {
                DataObject = data
            });
        }
    }
}