using Allure.Net.Commons;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using FarvaterWeb.Configuration;
using FarvaterWeb.ApiServices;
using Microsoft.Playwright;
using Serilog;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using AllureStatus = Allure.Net.Commons.Status; // Алиас для разрешения конфликта

namespace FarvaterWeb.Base;

public abstract class BaseTest : IAsyncLifetime
{
    protected IPlaywright PlaywrightInstance;
    protected IAPIRequestContext ApiRequest;
    protected BaseApiService Api;
    //protected AllureLifecycle Allure => AllureLifecycle.Instance;

    private static readonly string ProjectRoot =
        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

    public static string ScreenshotsPath = Path.Combine(ProjectRoot, "TestResults", "Screenshots");
    public static string VideoPath = Path.Combine(ProjectRoot, "TestResults", "Videos");
    public static string ReportsPath = Path.Combine(ProjectRoot, "TestResults", "TestReports");
    public static readonly string ReportFile = Path.Combine(ReportsPath,
        $"Run_{DateTime.Now:yyyyMMdd_HHmm}.html");

    private static ExtentReports _extent = null!;
    protected ExtentTest _test = null!;

    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;
    protected ILogger Log = null!;

    private bool _testFailed = true;
    private string _allureTestUuid = null!;
    private readonly ITestOutputHelper _output;

    static BaseTest()
    {
        if (Directory.Exists(ScreenshotsPath))
        {
            foreach (var file in Directory.GetFiles(ScreenshotsPath))
            {
                try { File.Delete(file); } catch { }
            }
        }

        Directory.CreateDirectory(ScreenshotsPath);
        Directory.CreateDirectory(VideoPath);
        Directory.CreateDirectory(ReportsPath);

        var spark = new ExtentSparkReporter(ReportFile);
        _extent = new ExtentReports();
        _extent.AttachReporter(spark);
    }

    protected BaseTest(ITestOutputHelper output)
    {
        _output = output; // Сохраняем для использования в InitializeAsync
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.TestOutput(output)
            .CreateLogger();

        Log = Serilog.Log.Logger;
        _test = _extent.CreateTest(GetType().Name);

        ///*// ДИАГНОСТИКА ALLURE
        //Log.Information("=== ALLURE DIAGNOSTICS ===");
        //Log.Information($"Allure Results Directory: {AllureLifecycle.Instance.ResultsDirectory}");
        //Log.Information($"Current Directory: {Directory.GetCurrentDirectory()}");
        //Log.Information($"Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");

        //var allureDir = AllureLifecycle.Instance.ResultsDirectory;
        //Log.Information($"Allure Directory Exists: {Directory.Exists(allureDir)}");

        //try
        //{
        //    Directory.CreateDirectory(allureDir);
        //    Log.Information($"Allure Directory Created/Verified: {allureDir}");
        //}
        //catch (Exception ex)
        //{
        //    Log.Error($"Failed to create Allure directory: {ex.Message}");
        //}

        //var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allureConfig.json");
        //Log.Information($"Config Path: {configPath}");
        //Log.Information($"Config Exists: {File.Exists(configPath)}");

        //if (File.Exists(configPath))
        //{
        //    var configContent = File.ReadAllText(configPath);
        //    Log.Information($"Config Content: {configContent}");
        //}
    }

    public async Task InitializeAsync()
    {
        Log.Information("[Setup] Подготовка папки скриншотов: {Path}", ScreenshotsPath);

        if (!Directory.Exists(VideoPath))
        {
            Directory.CreateDirectory(VideoPath);
        }

        BaseComponent.ResetCounter();

        // === ВЫЗОВ ALLURE СЕРВИСА ===
        _allureTestUuid = Guid.NewGuid().ToString();
        // Просто передаем UUID и имя класса теста
        //AllureService.StartTest(_allureTestUuid, GetType().Name);
        AllureService.StartTest(_allureTestUuid, GetType().Name, GetType().FullName);

        var playwright = await Playwright.CreateAsync();
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = ConfigurationReader.Headless,
            Args = new[] {
            "--disable-notifications",
            "--disable-device-discovery-notifications",
            "--no-sandbox",
            "--disable-setuid-sandbox"
        }
        });

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = VideoPath,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            Permissions = new[] { "notifications" } // Авто-отмена запросов на уведомления
        });

        Page = await Context.NewPageAsync();

        Log.Information("--- Начало теста: {TestName} ---", GetType().Name);

        PlaywrightInstance = await Playwright.CreateAsync();

        ApiRequest = await PlaywrightInstance.APIRequest.NewContextAsync(new()
        {
            BaseURL = "http://твой-адрес-фарватера.ru/",
            // Сюда можно добавить дефолтные заголовки
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Accept", "application/json" }
            }
        });

        Api = new BaseApiService(ApiRequest);
    }

    protected async Task Step(string name, Func<Task> action)
    {
      await AllureService.Step(name, action);
    }

    public async Task DisposeAsync()
    {
            string? videoPath = null;

            try
            {
            // 1. Обработка скриншота при падении
            if (_testFailed && Page != null)
            {
                var fileName = $"ERROR_{GetType().Name}_{DateTime.Now:HHmmss}.png";
                var path = Path.Combine(ScreenshotsPath, fileName);
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });

                // Отправляем в ExtentReports
                _test.Fail("<b><font color='red'>ТЕСТ ПРЕРВАН ОШИБКОЙ</font></b>",
                    MediaEntityBuilder.CreateScreenCaptureFromPath(path).Build());

                // Отправляем в Allure через сервис
                AllureService.AddAttachment("Скриншот ошибки", path); 
            }

            // 2. Получаем путь к видео до закрытия контекста
            if (Page?.Video != null)
            {
                videoPath = await Page.Video.PathAsync();
            }
            }
            catch (Exception ex)
            {
                Log.Error("[Dispose] Ошибка при сохранении артефактов: {Msg}", ex.Message);
            }
            finally
            {
            // 3. Закрываем браузер (нужно для финализации видеофайла)
            if (Page != null) await Page.CloseAsync();
            if (Context != null) await Context.DisposeAsync();
            if (Browser != null) await Browser.DisposeAsync();

            // 4. Прикрепляем видео, если оно создано
            if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
            {
                _test.Info($"<a href='file:///{videoPath}'>Запись видео теста</a>");
                AllureService.AddAttachment("Запись теста", videoPath);
            }

            // 5. Финализируем отчеты
            AllureService.Finish(_testFailed); // Наш "тихий" метод
            _extent.Flush(); // Основной HTML отчет

            Log.Information("--- Завершение работы браузера ---");
        }

        if (ApiRequest != null) await ApiRequest.DisposeAsync();
        if (PlaywrightInstance != null) PlaywrightInstance.Dispose();
    }

    protected void MarkTestAsPassed()
    {
        _testFailed = false;
        _test.Pass("Тест завершен успешно");
    }

    protected async Task LoginAsAdmin()
    {
        Log.Information("[Setup] Начало авторизации под SYSADMIN");

         await Page.GotoAsync(ConfigurationReader.BaseUrl);

        await Page.GetByPlaceholder("Пользователь").FillAsync("SYSADMIN");
        await Page.GetByPlaceholder("Пароль").FillAsync("");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Войти" }).ClickAsync();

        await Page.WaitForURLAsync("**/dashboard");

        _test.Info("Авторизация выполнена успешно");
        Log.Information("[Setup] Авторизация успешна");
    }
}