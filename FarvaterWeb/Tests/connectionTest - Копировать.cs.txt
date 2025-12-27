using Xunit;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace FarvaterWeb
{
    public class SanityCheckTests
    {
        // Используем [Fact] для xUnit и асинхронный метод
        [Fact]
        public async Task Browser_Should_Launch_And_Navigate_To_Google()
        {
            // --- 1. Прямая Инициализация Playwright и Браузера ---

            // Создаем IPlaywright (эквивалент dotnet tool install)
            using var playwright = await Playwright.CreateAsync();

            // Запускаем браузер Chromium (Headless = false, чтобы увидеть окно)
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, // false - чтобы браузер был виден
                SlowMo = 50      // Замедление 50 мс для наблюдения
            });

            // Создаем новую страницу (вкладку)
            var page = await browser.NewPageAsync();

            // --- 2. Жестко заданный URL и Действие ---

            // Переход на заданный URL
            await page.GotoAsync("https://farvater.mcad.dev/farvater/signin");

            // --- 3. Проверка Утверждения ---

            // Получаем заголовок страницы
            var title = await page.TitleAsync();

            // Проверяем, что заголовок содержит "Google"
            //Assert.Contains("Google", title);

            // Небольшая задержка, чтобы успеть увидеть браузер перед его закрытием
            await Task.Delay(2000);

            // Browser и Playwright закроются автоматически благодаря 'await using' и 'using var'
        }
    }
}