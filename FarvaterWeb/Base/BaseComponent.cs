using AventStack.ExtentReports;
using AventStack.ExtentReports.Model;
using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using Serilog;
using System.Xml.Linq;
using FarvaterWeb.Extensions;

namespace FarvaterWeb.Base;

public abstract class BaseComponent
{
    protected readonly IPage Page;
    protected readonly ILogger Log;
    protected readonly ILocator? Root;
    protected readonly string _componentName;
    protected readonly ExtentTest _test;

    private static int _stepCounter = 0;

    public static void ResetCounter()
    {
        _stepCounter = 0;
    }

    protected BaseComponent(IPage page, ILogger logger, ExtentTest test, string componentName = "Component")
    {
        Page = page;
        Log = logger;
        _test = test;
        //_componentName = componentName;
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

    protected async Task Do(string stepName, Func<Task> action)
    {
        Log?.Information(stepName);
        _test?.Info(stepName);
        await AllureService.Step(stepName, action);
    }

    protected async Task<T> Do<T>(string stepName, Func<Task<T>> action)
    {
        Log?.Information(stepName);
        _test?.Info(stepName);
        return await AllureService.Step(stepName, action);
    }

    protected async Task AutoScreenshot(string actionName)
    {
        _stepCounter++;
        var fileName = $"step_{_stepCounter:D3}_{_componentName}_{actionName}.png";
        var path = Path.Combine(BaseTest.ScreenshotsPath, fileName);

        Directory.CreateDirectory(BaseTest.ScreenshotsPath);
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });

        var relativePath = $"../Screenshots/{fileName}";
        _test?.Info($"Шаг {_stepCounter}: {actionName}",
            MediaEntityBuilder.CreateScreenCaptureFromPath(relativePath).Build());

        AllureService.AddAttachment(actionName, path);
    }

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
        (//*[contains(translate(normalize-space(text()), '{chars}', '{lower}'), '{lowerLabel}')]
        /ancestor::div[1]//input | 
        //*[contains(translate(normalize-space(text()), '{chars}', '{lower}'), '{lowerLabel}')]
        /following-sibling::input |
        //label[contains(translate(normalize-space(string(.)), '{chars}', '{lower}'), '{lowerLabel}')]//input)[1]";

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
        await Do($"[{_componentName}] Ввод '{text}' в поле '{name}'", async () =>
        {
            string logValue = maskText ? "****" : text;
            //Log.Information("[{Component}] Ввод '{Value}' в поле '{Name}'", _componentName, logValue, name);
            await locator.FillAsync(text);
            await AutoScreenshot($"Fill_{name.Replace(" ", "_")}");
        });
    }

    protected async Task DoFillByLabel(string label, string text)
    {
        // Находим локатор динамически
        var locator = GetInputByLabel(label);

        // Используем ваш существующий метод логирования и скриншотов
        await DoFill(locator, label, text);
    }

    /*public async Task SetCheckboxByText(string label, bool state)
    {
        await Do($"{(state ? "Установка" : "Снятие")} чек-бокса '{label}'", async () =>
        {
            var checkbox = Page.GetByLabel(label);
            if (state)
                await checkbox.CheckAsync();
            else
                await checkbox.UncheckAsync();
        });
    }*/

    public async Task SetCheckboxByText(string label, bool state)
    {
        await Do($"{(state ? "Установка" : "Снятие")} чек-бокса '{label}'", async () =>
        {
            var checkbox = Page.GetByLabel(label);

            // 1. Выполняем действие
            if (state)
                await checkbox.CheckAsync();
            else
                await checkbox.UncheckAsync();

            // 2. ПРОВЕРКА (Assertion)
            // Метод ToBeCheckedAsync сам подождет (по умолчанию до 5 сек), 
            // пока чекбокс примет нужное состояние.
            await Assertions.Expect(checkbox).ToBeCheckedAsync(new() { Checked = state });
        });
    }


    public async Task SetCheckboxInRow(ILocator rowLocator, bool state)
    {
        await Do($"{(state ? "Выбор" : "Снятие выбора")} в строке", async () =>
        {
            // Ищем внутри строки элемент типа checkbox
            var checkbox = rowLocator.Locator("input[type='checkbox'], .v-checkbox");

            if (state)
                await checkbox.CheckAsync(new() { Force = true });
            else
                await checkbox.UncheckAsync(new() { Force = true });

            // 2. ПРОВЕРКА (Assertion)
            // Метод ToBeCheckedAsync сам подождет (по умолчанию до 5 сек), 
            // пока чекбокс примет нужное состояние.
            await Assertions.Expect(checkbox).ToBeCheckedAsync(new() { Checked = state });
        });    
    }

    protected async Task<string> DoGetText(ILocator locator, string name)
    {
        // Do<string> вызовет AllureService.Step<string>
        // Тот получит текст из Playwright и вернет его сюда
        return await Do($"[{_componentName}] Получение текста из '{name}'", async () =>
        {
            return await locator.InnerTextAsync();
        });
    }

    // --- Клики ---
    protected async Task DoClick(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Клик по '{name}'", async () =>
        //Log.Information("[{Component}] Клик по '{Name}'", _componentName, name);
        {
            await locator.ClickAsync();
            await AutoScreenshot($"Click_{name.Replace(" ", "_")}");
        });

    }

    protected async Task DoDoubleClick(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Клик по '{name}'", async () =>
        //Log.Information("[{Component}] Двойной клик по '{Name}'", _componentName, name);
        {
            await locator.DblClickAsync();
            await AutoScreenshot($"Click_{name.Replace(" ", "_")}");
        });

    }

    

    protected async Task DoClickByText(string buttonText)
    {
        // 1. Создаем локатор через роли (ищет button, a[role=button], input[type=submit] и т.д.)
        // Exact = false заменяет вашу логику с translate (игнорирует регистр)
        var locator = Page.GetByRole(AriaRole.Button, new()
        {
            Name = buttonText,
            Exact = false
        }).First;

        // 2. Оборачиваем в наш мастер-метод Do
        await Do($"[{_componentName}] Нажатие на кнопку '{buttonText}'", async () =>
        {
            // Скроллим и проверяем видимость (стандарт надежности)
            await locator.ScrollIntoViewIfNeededAsync();
            await Assertions.Expect(locator).ToBeVisibleAsync(new() { Timeout = 7000 });

            // Кликаем
            await locator.ClickAsync(new() { Force = true });

            // Скриншот
            await AutoScreenshot($"Click_{buttonText.Replace(" ", "_")}");
        });
    }

    /*protected ILocator Button(string text) =>
    Page.GetByRole(AriaRole.Button, new() { Name = text, Exact = false }).First;*/

    // Для кнопок, где текст — это часть интерфейса (самый частый случай)
    /*protected ILocator ButtonWithText(string text) =>
        Page.GetByRole(AriaRole.Button, new() { Name = text, Exact = false });*/

    protected SmartLocator ButtonWithText(string text) =>
    new SmartLocator(
        Page.GetByRole(AriaRole.Button, new() { Name = text }),
        text,
        "Кнопка",
        _componentName, // Используем твое поле
        Page);

    // Для кнопок, где текст привязан через тег <label> или aria-label
    protected ILocator ButtonWithLabel(string label) =>
        Page.GetByLabel(label);

    // Универсальный метод для кнопок по CSS/XPath (иконки, классы, ID)
    protected ILocator Button(string selector) =>
        Page.Locator(selector);


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
        // Используем Do для логирования в Serilog, Allure и Extent одним махом
        await Do($"[{_componentName}] Наведение мыши на '{name}'", async () =>
        {
            // Выполняем само действие
            await locator.HoverAsync();

            // Делаем скриншот внутри шага
            await AutoScreenshot($"Hover_{name.Replace(" ", "_")}");
        });
    }

    // --- Перетаскивание (Drag and Drop) ---
    protected async Task DoDragAndDrop(ILocator source, string sourceName, ILocator target, string targetName)
    {
        await Do($"[{_componentName}] Перетаскивание '{sourceName}' на '{targetName}'", async () =>
        {
            await source.DragToAsync(target);
            await AutoScreenshot($"Drag_{sourceName}_to_{targetName}");
        });
    }

    // --- Выпадающие списки ---
    protected async Task DoSelectOption(ILocator locator, string name, string label)
    {
        await Do($"[{_componentName}] Выбор '{label}' в списке '{name}'", async () =>
        {
            // Выбираем опцию по тексту (Label)
            await locator.SelectOptionAsync(new SelectOptionValue { Label = label });
            await AutoScreenshot($"Select_{name.Replace(" ", "_")}");
        });
    }

    // --- Скролл к элементу ---
    protected async Task DoScrollToElement(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Прокрутка к '{name}'", async () =>
        {
            await locator.ScrollIntoViewIfNeededAsync();
            // Скриншот после скролла подтвердит, что элемент в поле зрения
            await AutoScreenshot($"Scroll_{name.Replace(" ", "_")}");
        });
    }

    // --- Проверки (Assertions) ---
    protected async Task AssertIsVisible(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Проверка видимости: '{name}'", async () =>
        {
            await Assertions.Expect(locator).ToBeVisibleAsync();
        });
    }

    protected async Task AssertNotVisible(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Проверка отсутствия: '{name}'", async () =>
        {
            await Assertions.Expect(locator).ToBeHiddenAsync();
        });
    }

    protected async Task AssertIsEnabled(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Проверка доступности (Enabled): '{name}'", async () =>
        {
            await Assertions.Expect(locator).ToBeEnabledAsync();
        });
    }

    protected async Task AssertElementText(ILocator locator, string expectedText, string name)
    {
        await Do($"[{_componentName}] Проверка текста элемента '{name}': ожидается '{expectedText}'", async () =>
        {
            await Assertions.Expect(locator).ToHaveTextAsync(expectedText);
        });
    }

    protected async Task AssertTextExists(string text, bool exact = false)
    {
        // Используем ваш мастер-метод Do для отчетов
        await Do($"[{_componentName}] Проверка наличия текста: '{text}'", async () =>
        {
            var locator = Page.GetByText(text, new() { Exact = exact }).First;

            // Стандартный ассерт Playwright с ожиданием
            await Assertions.Expect(locator).ToBeVisibleAsync(new() { Timeout = 10000 });

            // Ваш метод для скриншота
            await AutoScreenshot($"VerifyText_{text.Replace(" ", "_")}");
        });
    }

    protected bool MakeStepScreenshots = true;

    /*protected async Task AutoScreenshot(string actionName)
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
    }*/

    public async Task SelectDropdownItemByNumber(string label, int number)
    {
        // Мы передаем (number - 1), потому что для пользователя первый пункт — это 1,
        // а для программиста (и Playwright) — это индекс 0.
        await Do($"Выбор пункта №{number} в списке '{label}'",
            async () => await Page.GetByLabel(label).SelectByIndexAndVerifyAsync(number - 1));
    }
}