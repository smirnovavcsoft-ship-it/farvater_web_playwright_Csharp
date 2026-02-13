using AventStack.ExtentReports;
using AventStack.ExtentReports.Model;
using FarvaterWeb.Components;
using FarvaterWeb.Extensions;
using FarvaterWeb.ApiServices;
using Microsoft.Playwright;
using Serilog;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FarvaterWeb.Base;

public abstract class BaseComponent
{
    protected readonly IPage Page;
    protected readonly ILogger Log;
    protected readonly ILocator? Root;
    protected readonly string _componentName;
    protected readonly ExtentTest _test;
    protected readonly BaseApiService Api;
    

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
        Api = new BaseApiService(page.APIRequest);
    }

    protected DropdownComponent Dropdown => new DropdownComponent(Page, _componentName);

    protected InputComponent Input => new InputComponent(Page, _componentName);

    protected DescriptionComponent Description => new DescriptionComponent(Page, _componentName);

    protected DateComponent Date => new DateComponent(Page, Log, _test, _componentName);

    protected RangeComponent Range => new RangeComponent(Page, Log, _test, _componentName);

    protected ButtonComponent Button => new ButtonComponent(Page, _componentName);

    
    protected CheckboxComponent Checkbox(string label) => new CheckboxComponent(Page, label, _componentName);

    // --- Вспомогательный метод для поиска локаторов внутри компонента ---
    protected ILocator GetLocator(ILocator locator) => Root ?? locator;

    //Компоненты для элементов

    
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

    /*protected async Task AutoScreenshot(string actionName)
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
    }*/

    protected async Task AutoScreenshot(string actionName)
    {
        _stepCounter++;

        // 1. Очищаем имя действия от запрещенных символов (*, :, /, \, и т.д.)
        string safeActionName = actionName;
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            safeActionName = safeActionName.Replace(c, '_');
        }

        // 2. Формируем имя файла, используя ОЧИЩЕННОЕ имя
        var fileName = $"step_{_stepCounter:D3}_{_componentName}_{safeActionName}.png";
        var path = Path.Combine(BaseTest.ScreenshotsPath, fileName);

        // 3. Создаем директорию и делаем скриншот
        Directory.CreateDirectory(BaseTest.ScreenshotsPath);
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });

        // 4. Логирование в отчеты (здесь можно оставить оригинальное actionName для красоты)
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

        // XPath в его исходном виде
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

    protected async Task DoFillByLabel1(string label, string text)
    {
        // ХАРДКОД: Игнорируем label и ищем напрямую по подсказке в инпуте
        // [placeholder*='наименование'] — найдет поле, где в подсказке есть это слово
        var locator = Page.Locator("input[placeholder*='Введите наименование'], textarea[placeholder*='Введите наименование']").First;

        await DoFill(locator, label, text);
    }
       

    public async Task SetCheckboxByText(string label, bool state)
    {
        await Do($"{(state ? "Установка" : "Снятие")} чек-бокса '{label}'", async () =>
        {
            /*var checkbox = Page.GetByLabel(label);
             

            // 1. Выполняем действие
            if (state)
                await checkbox.CheckAsync();
            else
                await checkbox.UncheckAsync();*/

            //var checkbox = Checkbox(label);
            await Checkbox(label).SetAsync(state);

            // 2. ПРОВЕРКА (Assertion)
            // Метод ToBeCheckedAsync сам подождет (по умолчанию до 5 сек), 
            // пока чекбокс примет нужное состояние.
            //await Assertions.Expect(checkbox).ToBeCheckedAsync(new() { Checked = state });
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
    /*protected ILocator Button(string selector) =>
        Page.Locator(selector);*/


   

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

    

    
}