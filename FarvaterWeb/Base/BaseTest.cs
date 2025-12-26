using Microsoft.Playwright;
using Xunit;
using Serilog;
using Xunit.Abstractions;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace FarvaterWeb.Base; // Убедитесь, что namespace совпадает с папкой

public abstract class BaseTest : IAsyncLifetime
{
    private static readonly string ProjectRoot = 
        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

    public static string ScreenshotsPath = Path.Combine(ProjectRoot, "TestResults", "Screenshots");
    public static string VideoPath = Path.Combine(ProjectRoot, "TestResults", "Videos");
    public static string ReportsPath = Path.Combine(ProjectRoot, "TestResults", "TestReports");
    //public static string ReportFile = Path.Combine(ReportsPath, "Index.html");
    // Генерируем имя файла один раз при старте приложения
    // Формат: Run_20251220_1430.html
    public static readonly string ReportFile = Path.Combine(ReportsPath,
        $"Run_{DateTime.Now:yyyyMMdd_HHmm}.html");

    private static ExtentReports _extent = null!;
    protected ExtentTest _test = null!; // Тест для текущего прогона

    // Используем null!, чтобы убрать предупреждения CS8618
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;
    protected ILogger Log = null!;

    private bool _testFailed = true;



    // 1. Статический конструктор для инициализации ExtentReports (1 раз на все тесты)
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


    // 2. Обычный конструктор для Serilog и создания записи теста
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
        // Создаем ветку в отчете для конкретного тест-класса
        _test = _extent.CreateTest(GetType().Name);
    }

    public async Task InitializeAsync()
    {
        Log.Information("[Setup] Подготовка папки скриншотов: {Path}", ScreenshotsPath);

        // 1. Очистка скриншотов
        //if (Directory.Exists(ScreenshotsPath))
        //{
            //foreach (var file in Directory.GetFiles(ScreenshotsPath))
            //{
                //try { File.Delete(file); } catch { /* пропуск заблокированных */ }
            //}
        //}
        //else
        //{
            //Directory.CreateDirectory(ScreenshotsPath);
        //}

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
        // 1. ОБРАБОТКА ОШИБКИ (Вставляем в самое начало)
        // Если флаг _testFailed остался true, значит MarkTestAsPassed() не был вызван
        if (_testFailed && Page != null)
        {
            try
            {
                var fileName = $"ERROR_{GetType().Name}_{DateTime.Now:HHmmss}.png";
                var path = Path.Combine(ScreenshotsPath, fileName);

                // Делаем финальный скриншот
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });

                var relativePath = $"../Screenshots/{fileName}";

                // Помечаем в ExtentReports как Fail, прикладываем скрин и вешаем категорию
                _test.Fail("<b><font color='red'>ТЕСТ ПРЕРВАН ОШИБКОЙ</font></b>",
                    MediaEntityBuilder.CreateScreenCaptureFromPath(relativePath).Build());

                _test.AssignCategory("Runtime Failures"); // Категория по умолчанию для упавших тестов
                Log.Error("[Dispose] Тест упал. Скриншот ошибки сохранен: {Path}", path);
            }
            catch (Exception ex)
            {
                Log.Error("[Dispose] Не удалось сохранить скриншот ошибки: {Msg}", ex.Message);
            }
        }

        // 2. РАБОТА С ВИДЕО (Ваш существующий код)
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
            // Опционально: можно добавить ссылку на видео в отчет
            _test.Info($"<a href='file:///{videoPath}'>Запись видео теста</a>");
        }

        Log.Information("--- Завершение работы браузера ---");
    }

    protected void MarkTestAsPassed()
    {
        _testFailed = false; // Мы подтверждаем, что тест дошел до конца без ошибок
        _test.Pass("Тест завершен успешно"); // Отмечаем в ExtentReports
    }

    protected async Task LoginAsAdmin()
    {
        Log.Information("[Setup] Начало авторизации под SYSADMIN");

        // Переход на страницу (BaseUrl можно взять из конфига или задать тут)
        await Page.GotoAsync("https://vash-sait.ru/signin");

        // Используем простые селекторы или те, что у вас в SignInPage
        await Page.FillAsync("input[name='login']", "SYSADMIN");
        await Page.FillAsync("input[name='password']", "ваш_пароль");
        await Page.ClickAsync("button[type='submit']");

        // Ждем, что мы попали на главную (Dashboard)
        await Page.WaitForURLAsync("**/dashboard");

        _test.Info("Авторизация выполнена успешно");
        Log.Information("[Setup] Авторизация успешна");
    }
}