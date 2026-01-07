using Microsoft.Playwright;
using Xunit;
using Serilog;
using Xunit.Abstractions;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Allure.Net.Commons;
using AllureStatus = Allure.Net.Commons.Status; // Алиас для разрешения конфликта

namespace FarvaterWeb.Base;

public abstract class BaseTest : IAsyncLifetime
{
    protected AllureLifecycle Allure => AllureLifecycle.Instance;

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
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.TestOutput(output)
            .CreateLogger();

        Log = Serilog.Log.Logger;
        _test = _extent.CreateTest(GetType().Name);

        // ДИАГНОСТИКА ALLURE
        Log.Information("=== ALLURE DIAGNOSTICS ===");
        Log.Information($"Allure Results Directory: {AllureLifecycle.Instance.ResultsDirectory}");
        Log.Information($"Current Directory: {Directory.GetCurrentDirectory()}");
        Log.Information($"Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");

        var allureDir = AllureLifecycle.Instance.ResultsDirectory;
        Log.Information($"Allure Directory Exists: {Directory.Exists(allureDir)}");

        try
        {
            Directory.CreateDirectory(allureDir);
            Log.Information($"Allure Directory Created/Verified: {allureDir}");
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to create Allure directory: {ex.Message}");
        }

        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allureConfig.json");
        Log.Information($"Config Path: {configPath}");
        Log.Information($"Config Exists: {File.Exists(configPath)}");

        if (File.Exists(configPath))
        {
            var configContent = File.ReadAllText(configPath);
            Log.Information($"Config Content: {configContent}");
        }
    }

    public async Task InitializeAsync()
    {
        Log.Information("[Setup] Подготовка папки скриншотов: {Path}", ScreenshotsPath);

        if (!Directory.Exists(VideoPath))
        {
            Directory.CreateDirectory(VideoPath);
        }

        BaseComponent.ResetCounter();

        // === ЯВНАЯ ИНИЦИАЛИЗАЦИЯ ALLURE ===
        _allureTestUuid = Guid.NewGuid().ToString();
        var testName = GetType().Name;

        Log.Information($"Creating Allure test case: {testName} with UUID: {_allureTestUuid}");

        try
        {
            var testResult = new TestResult
            {
                uuid = _allureTestUuid,
                name = testName,
                fullName = GetType().FullName,
                labels = new List<Label>
                {
                    new Label { name = "host", value = Environment.MachineName },
                    new Label { name = "thread", value = Environment.CurrentManagedThreadId.ToString() }
                }
            };

            AllureLifecycle.Instance.StartTestCase(testResult);
            Log.Information("Allure test case started successfully");
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to start Allure test case: {ex.Message}");
            Log.Error($"Stack trace: {ex.StackTrace}");
        }

        var playwright = await Playwright.CreateAsync();
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = VideoPath,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        Page = await Context.NewPageAsync();

        Log.Information("--- Начало теста: {TestName} ---", GetType().Name);
    }

    public async Task DisposeAsync()
    {
        // === ЗАВЕРШЕНИЕ ALLURE ===
        if (!string.IsNullOrEmpty(_allureTestUuid))
        {
            try
            {
                Log.Information($"Finalizing Allure test case: {_allureTestUuid}");

                // Получаем TestResultContainer напрямую
                var testResultContainer = new TestResultContainer
                {
                    uuid = Guid.NewGuid().ToString()
                };

                // Обновляем тест напрямую через файл
                var allureDir = AllureLifecycle.Instance.ResultsDirectory;
                var testResultFile = Path.Combine(allureDir, $"{_allureTestUuid}-result.json");

                // Проверяем, создан ли файл результата
                if (File.Exists(testResultFile))
                {
                    Log.Information($"Test result file found: {testResultFile}");

                    // Читаем существующий результат
                    var json = File.ReadAllText(testResultFile);
                    var testResult = System.Text.Json.JsonSerializer.Deserialize<TestResult>(json);

                    if (testResult != null)
                    {
                        // Обновляем статус
                        testResult.status = _testFailed ? AllureStatus.failed : AllureStatus.passed;
                        testResult.statusDetails = _testFailed
                            ? new StatusDetails { message = "Test failed" }
                            : new StatusDetails { message = "Test passed" };

                        // Записываем обратно
                        var updatedJson = System.Text.Json.JsonSerializer.Serialize(testResult,
                            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(testResultFile, updatedJson);

                        Log.Information("Allure test status updated successfully");
                    }
                }
                else
                {
                    Log.Warning($"Test result file not found: {testResultFile}");
                }

                // Проверяем файлы
                if (Directory.Exists(allureDir))
                {
                    var files = Directory.GetFiles(allureDir);
                    Log.Information($"Files in Allure directory: {files.Length}");
                    foreach (var file in files)
                    {
                        Log.Information($"  - {Path.GetFileName(file)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to finalize Allure test case: {ex.Message}");
                Log.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        // 1. ОБРАБОТКА ОШИБКИ
        if (_testFailed && Page != null)
        {
            try
            {
                var fileName = $"ERROR_{GetType().Name}_{DateTime.Now:HHmmss}.png";
                var path = Path.Combine(ScreenshotsPath, fileName);

                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });

                if (File.Exists(path))
                {
                    var attachmentGuid = Guid.NewGuid().ToString("N");
                    var attachmentFileName = $"{attachmentGuid}-attachment.png";
                    var attachmentPath = Path.Combine(AllureLifecycle.Instance.ResultsDirectory, attachmentFileName);

                    File.Copy(path, attachmentPath, true);
                    Log.Information($"Screenshot copied to Allure directory: {attachmentPath}");
                }

                var relativePath = $"../Screenshots/{fileName}";
                _test.Fail("<b><font color='red'>ТЕСТ ПРЕРВАН ОШИБКОЙ</font></b>",
                    MediaEntityBuilder.CreateScreenCaptureFromPath(relativePath).Build());

                _test.AssignCategory("Runtime Failures");
                Log.Error("[Dispose] Тест упал. Скриншот ошибки сохранен: {Path}", path);
            }
            catch (Exception ex)
            {
                Log.Error("[Dispose] Не удалось сохранить скриншот ошибки: {Msg}", ex.Message);
            }
        }

        // 2. РАБОТА С ВИДЕО
        string? videoPath = null;
        if (Page != null && Page.Video != null)
        {
            videoPath = await Page.Video.PathAsync();
        }

        // 3. ЗАКРЫТИЕ БРАУЗЕРА
        if (Page != null) await Page.CloseAsync();
        if (Context != null) await Context.DisposeAsync();
        if (Browser != null) await Browser.DisposeAsync();

        // 4. СОХРАНЕНИЕ ОТЧЕТА
        _extent.Flush();

        if (videoPath != null)
        {
            Log.Information("[Video] Тест завершен. Видео сохранено: {Path}", videoPath);
            _test.Info($"<a href='file:///{videoPath}'>Запись видео теста</a>");

            await Task.Delay(500);
            if (File.Exists(videoPath))
            {
                try
                {
                    var attachmentGuid = Guid.NewGuid().ToString("N");
                    var attachmentFileName = $"{attachmentGuid}-attachment.webm";
                    var attachmentPath = Path.Combine(AllureLifecycle.Instance.ResultsDirectory, attachmentFileName);

                    File.Copy(videoPath, attachmentPath, true);
                    Log.Information($"Video copied to Allure directory: {attachmentPath}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to copy video: {ex.Message}");
                }
            }
        }

        Log.Information("--- Завершение работы браузера ---");
    }

    protected void MarkTestAsPassed()
    {
        _testFailed = false;
        _test.Pass("Тест завершен успешно");
    }

    protected async Task LoginAsAdmin()
    {
        Log.Information("[Setup] Начало авторизации под SYSADMIN");

        await Page.GotoAsync("https://farvater.mcad.dev/farvater/");

        await Page.GetByPlaceholder("Пользователь").FillAsync("SYSADMIN");
        await Page.GetByPlaceholder("Пароль").FillAsync("");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Войти" }).ClickAsync();

        await Page.WaitForURLAsync("**/dashboard");

        _test.Info("Авторизация выполнена успешно");
        Log.Information("[Setup] Авторизация успешна");
    }
}