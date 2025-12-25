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

    //Компоненты для элементов

    /*protected ILocator GetInputByLabel(string labelText)
    {
        // Этот XPath ищет input, который находится внутри того же контейнера, что и текст заголовка
        // Или input, на который ссылается тег <label>
        return Page.Locator($"//div[contains(text(), '{labelText}')]/following-sibling::input | //label[contains(text(), '{labelText}')]/..//input");
    }*/

    protected ILocator GetInputByLabel(string labelText)
    {
        // Приводим искомый текст к нижнему регистру для сравнения
        string lowerLabel = labelText.ToLower().Trim();

        // Алфавиты для замены (кириллица + латиница)
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        string lower = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        // XPath, который:
        // 1. Ищет элемент, содержащий текст (span, label, div)
        // 2. Очищает его от лишних пробелов
        // 3. Переводит в нижний регистр
        // 4. Находит ближайший input
        string xpath = $@"
        (//*[translate(normalize-space(text()), '{chars}', '{lower}') = '{lowerLabel}']
        /ancestor::div[1]//input | 
        //*[translate(normalize-space(text()), '{chars}', '{lower}') = '{lowerLabel}']
        /following-sibling::input |
        //label[translate(normalize-space(string(.)), '{chars}', '{lower}') = '{lowerLabel}']//input)[1]";

        return Page.Locator(xpath);
    }

    protected ILocator GetButtonByText(string buttonText)
    {
        string lowerButton = buttonText.ToLower().Trim();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        string lower = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        // Ищем любой элемент, который выглядит как кнопка или ссылка, 
        // чей текст после очистки пробелов совпадает с искомым (в нижнем регистре)
        string xpath = $@"
        ( //button[translate(normalize-space(.), '{chars}', '{lower}') = '{lowerButton}'] | 
          //input[(@type='button' or @type='submit') and translate(normalize-space(@value), '{chars}', '{lower}') = '{lowerButton}'] |
          //*[(@role='button' or contains(@class, 'btn')) and translate(normalize-space(.), '{chars}', '{lower}') = '{lowerButton}'] 
        )[1]";

        return Page.Locator(xpath);
    }

    // --- Действия с текстом ---
    protected async Task DoFill(ILocator locator, string name, string text, bool maskText = false)
    {
        string logValue = maskText ? "****" : text;
        Log.Information("[{Component}] Ввод '{Value}' в поле '{Name}'", _componentName, logValue, name);
        await locator.FillAsync(text);
        await AutoScreenshot($"Fill_{name.Replace(" ", "_")}");
    }

    protected async Task DoFillByLabel(string label, string text)
    {
        // Находим локатор динамически
        var locator = GetInputByLabel(label);

        // Используем ваш существующий метод логирования и скриншотов
        await DoFill(locator, label, text);
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

    /*protected async Task DoClickByText(string buttonText)
    {
        Log.Information("[{Component}] Нажатие на кнопку '{Text}'", _componentName, buttonText);

        var locator = GetButtonByText(buttonText);

        // Ждем, чтобы кнопка стала видимой и доступной для клика
        await Assertions.Expect(locator).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Assertions.Expect(locator).ToBeEnabledAsync();

        await locator.ClickAsync();

        // Делаем скриншот ПОСЛЕ клика, чтобы увидеть результат действия
        await AutoScreenshot($"Click_{buttonText.Replace(" ", "_")}");
    }*/

    /*protected async Task DoClickByText(string buttonText)
    {
        Log.Information("[{Component}] Нажатие на кнопку '{Text}'", _componentName, buttonText);

        // 1. Подготовка переменных для XPath
        string lowerLabel = buttonText.ToLower().Trim(); // Вот она, потеряшка!
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        string lower = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        // Формируем XPath для "всеядного" поиска
        //string xpathSelector = $"xpath=(//button|//a|//*[@role='button']|//input[@type='submit' or @type='button'])[translate(normalize-space(.), '{chars}', '{lower}') = '{lowerLabel}' or translate(normalize-space(@value), '{chars}', '{lower}') = '{lowerLabel}']";
        string xpathSelector = $"xpath=(//button|//a|//*[@role='button']|//input[@type='submit' or @type='button'])[contains(translate(., '{chars}', '{lower}'), '{lowerLabel}') or contains(translate(@value, '{chars}', '{lower}'), '{lowerLabel}')]";

        // 2. Инициализация локаторов
        var roleLocator = Page.GetByRole(AriaRole.Button, new() { Name = buttonText });

        // Объединяем: ищем либо по роли, либо по нашему хитрому XPath
        // Добавляем фильтр >> visible=true, чтобы не кликать по скрытым элементам
        var combinedLocator = Page.Locator(xpathSelector).Or(roleLocator).Filter(new() { Visible = true }).First;

        try
        {
            // 3. Ожидание и клик
            await Assertions.Expect(combinedLocator).ToBeVisibleAsync(new() { Timeout = 5000 });
            await combinedLocator.ClickAsync();

            await AutoScreenshot($"Click_{buttonText.Replace(" ", "_")}");
        }
        catch (Exception ex)
        {
            Log.Error("[{Component}] Не удалось кликнуть по кнопке '{Text}': {Error}", _componentName, buttonText, ex.Message);
            throw; // Пробрасываем ошибку в тест
        }
    }*/

    protected async Task DoClickByText(string buttonText)
    {
        Log.Information("[{Component}] Нажатие на кнопку '{Text}'", _componentName, buttonText);

        string lowerLabel = buttonText.ToLower().Trim();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        string lower = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        // 1. Формируем локатор
        string xpathSelector = $"xpath=(//button|//a|//*[@role='button']|//input[@type='submit' or @type='button'])[contains(translate(., '{chars}', '{lower}'), '{lowerLabel}') or contains(translate(@value, '{chars}', '{lower}'), '{lowerLabel}')]";
        var roleLocator = Page.GetByRole(AriaRole.Button, new() { Name = buttonText });
        var combinedLocator = Page.Locator(xpathSelector).Or(roleLocator).First;

        // 2. Улучшенная логика взаимодействия
        try
        {
            // Сначала пробуем проскроллить
            try
            {
                await combinedLocator.ScrollIntoViewIfNeededAsync(new() { Timeout = 2000 });
            }
            catch { /* Если не скроллится, возможно элемент уже в области видимости */ }

            // Ждем видимости
            await Assertions.Expect(combinedLocator).ToBeVisibleAsync(new() { Timeout = 7000 });

            // Кликаем (добавим Force, чтобы игнорировать возможные невидимые перекрытия)
            await combinedLocator.ClickAsync(new() { Force = true });

            await AutoScreenshot($"Click_{buttonText.Replace(" ", "_")}");
        }
        catch (Exception ex)
        {
            Log.Error("Не удалось кликнуть по кнопке '{Text}': {Error}", buttonText, ex.Message);
            throw;
        }
    }

    /*protected async Task DoClickByText(string buttonText)
    {
        Log.Information("[{Component}] Нажатие на кнопку (GetByRole) '{Text}'", _componentName, buttonText);

        // Ищем строго по роли кнопки с учетом имени (регистронезависимо по умолчанию)
        var locator = Page.GetByRole(AriaRole.Button, new() { Name = buttonText }).First;

        try
        {
            // Ждем, чтобы кнопка стала видимой (стандартные 5-7 секунд)
            await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 7000 });

            // Кликаем с параметром Force, чтобы пробить возможные невидимые перекрытия
            await locator.ClickAsync(new() { Force = true });

            await AutoScreenshot($"Click_{buttonText.Replace(" ", "_")}");
        }
        catch (Exception ex)
        {
            Log.Error("GetByRole не смог найти или кликнуть по кнопке '{Text}': {Error}", buttonText, ex.Message);

            // Если упало, сделаем диагностический скриншот сразу
            await AutoScreenshot($"ERROR_Click_{buttonText.Replace(" ", "_")}");
            throw;
        }
    }*/

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