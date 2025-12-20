using Microsoft.Playwright;
using Xunit;
using Serilog;
using Xunit.Abstractions;

namespace FarvaterWeb.Base; // Убедитесь, что namespace совпадает с папкой

public abstract class BaseTest : IAsyncLifetime
{
    public static string ScreenshotsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults", "Screenshots");
    // Используем null!, чтобы убрать предупреждения CS8618
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;
    protected ILogger Log = null!;

    private bool _testFailed = true;

    // В xUnit конструктор — это место для инициализации логгера через ITestOutputHelper
    protected BaseTest(ITestOutputHelper output)
    {
        // Настраиваем Serilog на вывод прямо в консоль xUnit
        // Для работы .WriteTo.TestOutput(output) нужен пакет Serilog.Sinks.XUnit
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.TestOutput(output)
            .CreateLogger();

        Log = Serilog.Log.Logger;
    }

    public async Task InitializeAsync()
    {
        //var absolutePath = Path.GetFullPath(ScreenshotsPath);
        Log.Information("[Setup] Подготовка папки скриншотов: {Path}", ScreenshotsPath);

        if (Directory.Exists(ScreenshotsPath))
        {
            foreach (var file in Directory.GetFiles(ScreenshotsPath))
            {
                try { File.Delete(file); } catch { /* пропуск заблокированных */ }
            }
        }
        else
        {
            Directory.CreateDirectory(ScreenshotsPath);
        }

        // 2. Сбрасываем счетчик в компонентах
        BaseComponent.ResetCounter();

        var playwright = await Playwright.CreateAsync();

        // Поставим Headless = false, чтобы при локальном запуске видеть браузер
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = "videos/",
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        Page = await Context.NewPageAsync();
        Log.Information("--- Начало теста: {TestName} ---", GetType().Name);

        if (Directory.Exists("screenshots"))
        {
            Directory.Delete("screenshots", true); // Удаляет старые скриншоты перед новым запуском
        }
    }

    public async Task DisposeAsync()
    {
        // Получаем путь к видео ДО закрытия контекста
        string? videoPath = null;
        if (Page != null && Page.Video != null)
        {
            videoPath = await Page.Video.PathAsync();
        }

        // Закрываем всё по порядку
        if (Page != null) await Page.CloseAsync();
        if (Context != null) await Context.DisposeAsync();
        if (Browser != null) await Browser.DisposeAsync();

        if (videoPath != null)
        {
            Log.Information("[Video] Тест завершен. Видео сохранено: {Path}", videoPath);
        }

        Log.Information("--- Завершение работы браузера ---");
    }

    protected void MarkTestAsPassed() => _testFailed = false;
}