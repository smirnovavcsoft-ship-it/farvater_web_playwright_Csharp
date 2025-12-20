using Microsoft.Playwright;
using Serilog;
using System.Xml.Linq;
using AventStack.ExtentReports;


namespace FarvaterWeb.Base;

public abstract class BaseComponent
{
    protected readonly IPage Page;
    protected readonly ILogger Log;
    protected readonly ILocator? Root;
    protected readonly string _componentName;
    protected readonly ExtentTest _extentTest;

    private static int _stepCounter = 0;

    public static void ResetCounter()
    {
        _stepCounter = 0;
    }

    protected BaseComponent(IPage page, ILogger logger, ExtentTest extentTest, ILocator? root = null)
    {
        Page = page;
        Log = logger;
        _extentTest = extentTest;
        Root = root;
        _componentName = GetType().Name; // Автоматически берет имя класса (например, "LoginForm")
    }

    // --- Вспомогательный метод для поиска локаторов внутри компонента ---
    protected ILocator GetLocator(ILocator locator) => Root ?? locator;

    // --- Действия с текстом ---
    protected async Task DoFill(ILocator locator, string name, string text, bool maskText = false)
    {
        string logValue = maskText ? "****" : text;
        Log.Information("[{Component}] Ввод '{Value}' в поле '{Name}'", _componentName, logValue, name);
        await locator.FillAsync(text);
        await AutoScreenshot($"Fill_{name.Replace(" ", "_")}");
    }

    protected async Task<string> DoGetText(ILocator locator, string name)
    {
        var text = await locator.InnerTextAsync();
        Log.Information("[{Component}] Получен текст из '{Name}': '{Text}'", _componentName, name, text);
        return text;
    }

    // --- Клики ---
    protected async Task DoClick(ILocator locator, string name)
    {
        Log.Information("[{Component}] Клик по '{Name}'", _componentName, name);
        await locator.ClickAsync();
        await AutoScreenshot($"Click_{name.Replace(" ", "_")}");

    }

    protected async Task DoDoubleClick(ILocator locator, string name)
    {
        Log.Information("[{Component}] Двойной клик по '{Name}'", _componentName, name);
        await locator.DblClickAsync();
        await AutoScreenshot($"DoubleClick_{name.Replace(" ", "_")}");
    }

    // --- Мышь и сложные действия ---
    protected async Task DoHover(ILocator locator, string name)
    {
        Log.Information("[{Component}] Наведение мыши на '{Name}'", _componentName, name);
        await locator.HoverAsync();
        await AutoScreenshot($"Hover_{name.Replace(" ", "_")}");
    }

    protected async Task DoDragAndDrop(ILocator source, string sourceName, ILocator target, string targetName)
    {
        Log.Information("[{Component}] Перетаскивание '{Source}' на '{Target}'", _componentName, sourceName, targetName);
        await source.DragToAsync(target);
        await AutoScreenshot($"Drag_{sourceName}_to_{targetName}");
    }

    // --- Выпадающие списки и скролл ---
    protected async Task DoSelectOption(ILocator locator, string name, string label)
    {
        Log.Information("[{Component}] Выбор значения '{Label}' в списке '{Name}'", _componentName, label, name);
        await locator.SelectOptionAsync(new SelectOptionValue { Label = label });
        await AutoScreenshot($"SelectOption_{name.Replace(" ", "_")}");
    }

    protected async Task DoScrollToElement(ILocator locator, string name)
    {
        Log.Information("[{Component}] Прокрутка к элементу '{Name}'", _componentName, name);
        await locator.ScrollIntoViewIfNeededAsync();
        await AutoScreenshot($"DoScrollToElement_{name.Replace(" ", "_")}");
    }

    // --- Проверки (Assertions) ---
    protected async Task AssertIsVisible(ILocator locator, string name)
    {
        Log.Information("[{Component}] Проверка видимости элемента '{Name}'", _componentName, name);
        await Assertions.Expect(locator).ToBeVisibleAsync();
    }

    protected async Task AssertNotVisible(ILocator locator, string name)
    {
        Log.Information("[{Component}] Проверка отсутствия элемента '{Name}'", _componentName, name);
        await Assertions.Expect(locator).ToBeHiddenAsync();
    }

    protected async Task AssertIsEnabled(ILocator locator, string name)
    {
        Log.Information("[{Component}] Проверка доступности элемента '{Name}'", _componentName, name);
        await Assertions.Expect(locator).ToBeEnabledAsync();
    }

    protected bool MakeStepScreenshots = true;

    protected async Task AutoScreenshot(string actionName)
    {
        if (!MakeStepScreenshots) return;
        _stepCounter++;

        // 1. Формируем имя и путь (используем ТОЛЬКО ScreenshotsPath)
        var fileName = $"step_{_stepCounter:D3}_{_componentName}_{actionName}.png";
        var path = Path.Combine(BaseTest.ScreenshotsPath, fileName);

        try
        {
            // 2. Создаем нужную директорию (TestResults/Screenshots)
            Directory.CreateDirectory(BaseTest.ScreenshotsPath);

            // 3. Сохраняем скриншот
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });

            // ПУТЬ ДЛЯ HTML: отчет лежит в TestReports/, а скрины в Screenshots/
            // Чтобы HTML увидел картинку, нужно выйти из своей папки и зайти в соседнюю
            var relativePath = $"../Screenshots/{fileName}";

            // Добавляем в отчет текст и скриншот
            _extentTest.Info($"Шаг {_stepCounter}: {actionName}",
                MediaEntityBuilder.CreateScreenCaptureFromPath(relativePath).Build());

            //Log.Information("[AutoScreenshot] Шаг {Step} сохранен в отчет", _stepCounter);

            // 4. Логируем результат
            //Log.Information("[AutoScreenshot] Шаг {Step} сохранен: {FullPath}", _stepCounter, path);
        }
        catch (Exception ex)
        {
            // Если скриншот не создался, вы увидите это в Output
            Log.Error("[AutoScreenshot] Критическая ошибка сохранения: {Message}", ex.Message);
        }
    }
}