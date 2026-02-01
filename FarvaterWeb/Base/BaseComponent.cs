using AventStack.ExtentReports;
using AventStack.ExtentReports.Model;
using FarvaterWeb.Components;
using FarvaterWeb.Extensions;
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
        _componentName = GetType().Name;       
    }

    protected DropdownComponent Dropdown => new DropdownComponent(Page, _componentName);

    protected InputComponent Input => new InputComponent(Page, _componentName);

    protected CalendarComponent Date => new CalendarComponent(Page, Log, _test, _componentName);

    protected CheckboxComponent Checkbox(string label) => new CheckboxComponent(Page, label, _componentName);

    protected ILocator GetLocator(ILocator locator) => Root ?? locator;

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

        string safeActionName = actionName;
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            safeActionName = safeActionName.Replace(c, '_');
        }

        var fileName = $"step_{_stepCounter:D3}_{_componentName}_{safeActionName}.png";
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
        string lowerLabel = labelText.ToLower().Trim();

        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        string lower = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

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

        string xpath = $@"
        ( //button[translate(normalize-space(.), '{chars}', '{lower}') = '{lowerButton}'] |
          //input[(@type='button' or @type='submit') and translate(normalize-space(@value), '{chars}', '{lower}') = '{lowerButton}'] |
          //*[(@role='button' or contains(@class, 'btn')) and translate(normalize-space(.), '{chars}', '{lower}') = '{lowerButton}']
        )[1]";

        return Page.Locator(xpath);
    }

    protected async Task DoFill(ILocator locator, string name, string text, bool maskText = false)
    {
        await Do($"[{_componentName}] Ввод '{text}' в поле '{name}'", async () =>
        {
            string logValue = maskText ? "****" : text;
            await locator.FillAsync(text);
            await AutoScreenshot($"Fill_{name.Replace(" ", "_")}");
        });
    }

    protected async Task DoFillByLabel(string label, string text)
    {
        var locator = GetInputByLabel(label);

        await DoFill(locator, label, text);
    }

    protected async Task DoFillByLabel1(string label, string text)
    {
        var locator = Page.Locator("input[placeholder*='Введите наименование'], textarea[placeholder*='Введите наименование']").First;

        await DoFill(locator, label, text);
    }

    public async Task SetCheckboxByText(string label, bool state)
    {
        await Do($"{(state ? "Установка" : "Снятие")} чек-бокса '{label}'", async () =>
        {
            await Checkbox(label).SetAsync(state);

        });
    }

    public async Task SetCheckboxInRow(ILocator rowLocator, bool state)
    {
        await Do($"{(state ? "Выбор" : "Снятие выбора")} в строке", async () =>
        {
            var checkbox = rowLocator.Locator("input[type='checkbox'], .v-checkbox");

            if (state)
                await checkbox.CheckAsync(new() { Force = true });
            else
                await checkbox.UncheckAsync(new() { Force = true });

            await Assertions.Expect(checkbox).ToBeCheckedAsync(new() { Checked = state });
        });
    }

    protected async Task<string> DoGetText(ILocator locator, string name)
    {
        return await Do($"[{_componentName}] Получение текста из '{name}'", async () =>
        {
            return await locator.InnerTextAsync();
        });
    }

    protected async Task DoClick(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Клик по '{name}'", async () =>
        {
            await locator.ClickAsync();
            await AutoScreenshot($"Click_{name.Replace(" ", "_")}");
        });
    }

    protected async Task DoDoubleClick(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Клик по '{name}'", async () =>
        {
            await locator.DblClickAsync();
            await AutoScreenshot($"Click_{name.Replace(" ", "_")}");
        });
    }

    protected async Task DoClickByText(string buttonText)
    {
        var locator = Page.GetByRole(AriaRole.Button, new()
        {
            Name = buttonText,
            Exact = false
        }).First;

        await Do($"[{_componentName}] Нажатие на кнопку '{buttonText}'", async () =>
        {
            await locator.ScrollIntoViewIfNeededAsync();
            await Assertions.Expect(locator).ToBeVisibleAsync(new() { Timeout = 7000 });

            await locator.ClickAsync(new() { Force = true });

            await AutoScreenshot($"Click_{buttonText.Replace(" ", "_")}");
        });
    }

    protected SmartLocator ButtonWithText(string text) =>
    new SmartLocator(
        Page.GetByRole(AriaRole.Button, new() { Name = text }),
        text,
        "Кнопка",
        _componentName,    
        Page);

    protected ILocator ButtonWithLabel(string label) =>
        Page.GetByLabel(label);

    protected ILocator Button(string selector) =>
        Page.Locator(selector);

    protected async Task DoHover(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Наведение мыши на '{name}'", async () =>
        {
            await locator.HoverAsync();

            await AutoScreenshot($"Hover_{name.Replace(" ", "_")}");
        });
    }

    protected async Task DoDragAndDrop(ILocator source, string sourceName, ILocator target, string targetName)
    {
        await Do($"[{_componentName}] Перетаскивание '{sourceName}' на '{targetName}'", async () =>
        {
            await source.DragToAsync(target);
            await AutoScreenshot($"Drag_{sourceName}_to_{targetName}");
        });
    }

    protected async Task DoSelectOption(ILocator locator, string name, string label)
    {
        await Do($"[{_componentName}] Выбор '{label}' в списке '{name}'", async () =>
        {
            await locator.SelectOptionAsync(new SelectOptionValue { Label = label });
            await AutoScreenshot($"Select_{name.Replace(" ", "_")}");
        });
    }

    protected async Task DoScrollToElement(ILocator locator, string name)
    {
        await Do($"[{_componentName}] Прокрутка к '{name}'", async () =>
        {
            await locator.ScrollIntoViewIfNeededAsync();
            await AutoScreenshot($"Scroll_{name.Replace(" ", "_")}");
        });
    }

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
        await Do($"[{_componentName}] Проверка наличия текста: '{text}'", async () =>
        {
            var locator = Page.GetByText(text, new() { Exact = exact }).First;

            await Assertions.Expect(locator).ToBeVisibleAsync(new() { Timeout = 10000 });

            await AutoScreenshot($"VerifyText_{text.Replace(" ", "_")}");
        });
    }

    protected bool MakeStepScreenshots = true;
}