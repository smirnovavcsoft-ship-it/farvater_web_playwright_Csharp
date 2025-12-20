using Microsoft.Playwright;
using Xunit;
using Serilog;
using Xunit.Abstractions;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace FarvaterWeb.Base; // Убедитесь, что namespace совпадает с папкой

public abstract class BaseTest : IAsyncLifetime
{
    public static string ScreenshotsPath = Path.Combine(
    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName,
    "TestResults",
    "Screenshots");

    public static string VideoPath = Path.Combine(
    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName,
    "TestResults",
    "Videos");

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
        Log.Information("[Setup] Подготовка папки скриншотов: {Path}", ScreenshotsPath);

        // 1. Очистка скриншотов
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

        // 2. Проверка папки для видео (без удаления файлов)
        if (!Directory.Exists(VideoPath))
        {
            Directory.CreateDirectory(VideoPath);
        }

        // 3. Сброс счетчика шагов
        BaseComponent.ResetCounter();

        // 4. Запуск Playwright и браузера
        var playwright = await Playwright.CreateAsync();
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

        // 5. Создание контекста (ТОЛЬКО ОДИН РАЗ с правильным путем для видео)
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = VideoPath,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        // 6. Открытие страницы
        Page = await Context.NewPageAsync();

        Log.Information("--- Начало теста: {TestName} ---", GetType().Name);
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