using Microsoft.Playwright;
using Serilog;

namespace MyProject.Base;

public abstract class BasePage : BaseComponent
{
    // Конструктор просто пробрасывает зависимости в BaseComponent
    // Мы передаем null в качестве Root, так как у страницы нет родительского локатора
    protected BasePage(IPage page, ILogger logger) : base(page, logger, null)
    {
    }

    /// <summary>
    /// Переход по URL с проверкой результата и логированием
    /// </summary>
    public async Task GoToUrl(string url, string expectedUrlPart)
    {
        Log.Information("[Navigation] Переход на URL: {Url}. Ожидаем часть пути: '{Part}'", url, expectedUrlPart);

        try
        {
            // Переходим на страницу
            var response = await Page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

            // Проверяем, не ответил ли сервер ошибкой
            if (response != null && !response.Ok)
            {
                Log.Warning("[Navigation] Сервер вернул статус: {Status}", response.Status);
            }

            // Ожидаем подтверждения навигации по URL
            await Page.WaitForURLAsync($"**{expectedUrlPart}**");
            Log.Information("[Navigation] Успешно достигнут URL: {CurrentUrl}", Page.Url);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[Navigation] ОШИБКА. Не удалось загрузить '{Part}'. Текущий URL: {Actual}",
                expectedUrlPart, Page.Url);

            await TakeScreenshotAsync($"Error_Nav_{expectedUrlPart.Replace("/", "_")}");
            throw;
        }
    }

    /// <summary>
    /// Создание скриншота всей страницы
    /// </summary>
    public async Task TakeScreenshotAsync(string name)
    {
        string fileName = $"{name}_{DateTime.Now:HHmmss}.png";
        string path = Path.Combine("screenshots", fileName);

        // Создаем папку, если она не существует
        Directory.CreateDirectory("screenshots");

        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });
        Log.Information("[Screenshot] Сохранен: {Path}", path);
    }

    /// <summary>
    /// Прокрутка всей страницы в самый низ (например, для подгрузки контента)
    /// </summary>
    public async Task ScrollToBottom()
    {
        Log.Information("[Page] Прокрутка страницы вниз");
        await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
    }

    /// <summary>
    /// Проверка заголовка вкладки браузера
    /// </summary>
    public async Task AssertPageTitle(string expectedTitle)
    {
        Log.Information("[Assertion] Проверка заголовка страницы: '{Expected}'", expectedTitle);
        await Assertions.Expect(Page).ToHaveTitleAsync(expectedTitle);
    }
}