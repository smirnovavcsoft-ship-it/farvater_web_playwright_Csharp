using Xunit;
using Microsoft.Playwright;
using System.Threading.Tasks;
using FarvaterWeb.Configuration;

namespace FarvaterWeb.Setup
{
    // Класс, который управляет жизненным циклом Playwright и Browser.
    // Реализует IAsyncLifetime для асинхронной инициализации (Setup/Teardown).
    public class PlaywrightFixture : IAsyncLifetime
    {
        // Эти объекты будут общими и доступны для всех тестов, использующих эту фикстуру
        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;
        public IPage Page { get; private set; } = null!;
        public string BaseUrl { get; private set; } = null!;

        /// <summary>
        /// Метод выполняется перед запуском всех тестов в классе.
        /// Инициализирует Playwright, запускает браузер и создает новую страницу.
        /// </summary>
        public async Task InitializeAsync()
        {
            // 1. Получаем настройки из нашего ConfigurationReader
            BaseUrl = ConfigurationReader.BaseUrl;
            string browserType = ConfigurationReader.BrowserType;
            bool headless = ConfigurationReader.Headless;

            // 2. Запуск Playwright
            _playwright = await Playwright.CreateAsync();

            // 3. Выбор и запуск браузера
            IBrowserType type = browserType.ToLower() switch
            {
                "chromium" => _playwright.Chromium,
                "firefox" => _playwright.Firefox,
                "webkit" => _playwright.Webkit,
                _ => _playwright.Chromium // По умолчанию Chromium
            };

            _browser = await type.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless
            });

            // 4. Создание новой страницы (IPage)
            Page = await _browser.NewPageAsync();
        }

        /// <summary>
        /// Метод выполняется после завершения всех тестов в классе.
        /// Закрывает браузер и освобождает ресурсы Playwright.
        /// </summary>
        public async Task DisposeAsync()
        {
            // Закрываем браузер, затем закрываем сам Playwright
            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}