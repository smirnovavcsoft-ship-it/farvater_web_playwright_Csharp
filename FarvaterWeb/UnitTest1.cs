using Xunit; // Используем пространство имен xUnit
using Microsoft.Playwright;
using System.Threading.Tasks; // Обязательно для асинхронных операций

namespace FarvaterWeb
{
    // Класс тестов: не требует специальных атрибутов, просто публичный класс
    public class BasicPlaywrightTests
    {
        // Атрибут [Fact] указывает, что это тестовый метод (без параметров)
        [Fact]
        public async Task Should_Navigate_To_PlaywrightPage_And_Check_Title()
        {
            // 1. Создание экземпляра Playwright
            using var playwright = await Playwright.CreateAsync();

            // 2. Запуск браузера (Chromium)
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                // Установите false для запуска видимого окна браузера
                Headless = true
            });

            // 3. Создание новой страницы
            var page = await browser.NewPageAsync();

            // 4. Переход на сайт
            await page.GotoAsync("https://playwright.dev/");

            // 5. Проверка утверждения: Используем Assert из xUnit
            var title = await page.TitleAsync();

            // Проверяем, что заголовок содержит 'Playwright'
            Assert.Contains("Playwright", title);
        }
    }
}