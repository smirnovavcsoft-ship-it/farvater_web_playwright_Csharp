using Microsoft.Playwright;
using Xunit;
using Serilog;

public abstract class BaseTest : IAsyncLifetime
{
    protected IBrowser Browser;
    protected IBrowserContext Context;
    protected IPage Page;
    protected ILogger Log;

    // В xUnit нет встроенного TestContext.CurrentContext.Result как в NUnit.
    // Для определения статуса теста обычно используются обертки или кастомные атрибуты,
    // но самый простой способ для видео — сохранять его всегда, либо удалять вручную.
    private bool _testFailed = true;

    public BaseTest()
    {
        // Инициализируем логгер (Serilog)
        Log = Serilog.Log.Logger;
    }

    // Заменяет [SetUp]
    public async Task InitializeAsync()
    {
        var playwright = await Playwright.CreateAsync();
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = "videos/"
        });

        Page = await Context.NewPageAsync();
        Log.Information("--- Запуск теста: инициализация браузера и контекста ---");
    }

    // Заменяет [TearDown]
    public async Task DisposeAsync()
    {
        // В xUnit сложно узнать результат теста прямо внутри DisposeAsync без сторонних библиотек.
        // Обычно видео сохраняют для всех тестов, а CI/CD чистит их, либо используют перехват исключений.

        var video = Page.Video;
        string? videoPath = null;

        if (video != null)
        {
            videoPath = await video.PathAsync();
        }

        await Page.CloseAsync();
        await Context.DisposeAsync();
        await Browser.DisposeAsync();

        if (videoPath != null)
        {
            Log.Information("[Video] Запись завершена. Файл: {Path}", videoPath);
        }
    }

    // Метод для пометки, что тест прошел успешно (вызывать в конце теста)
    protected void MarkTestAsPassed() => _testFailed = false;
}