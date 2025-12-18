using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.Base;

public abstract class BaseComponent
{
    protected readonly IPage Page;
    protected readonly ILogger Log;
    protected readonly ILocator? Root;
    protected readonly string _componentName;

    protected BaseComponent(IPage page, ILogger logger, ILocator? root = null)
    {
        Page = page;
        Log = logger;
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
    }

    protected async Task DoDoubleClick(ILocator locator, string name)
    {
        Log.Information("[{Component}] Двойной клик по '{Name}'", _componentName, name);
        await locator.DblClickAsync();
    }

    // --- Мышь и сложные действия ---
    protected async Task DoHover(ILocator locator, string name)
    {
        Log.Information("[{Component}] Наведение мыши на '{Name}'", _componentName, name);
        await locator.HoverAsync();
    }

    protected async Task DoDragAndDrop(ILocator source, string sourceName, ILocator target, string targetName)
    {
        Log.Information("[{Component}] Перетаскивание '{Source}' на '{Target}'", _componentName, sourceName, targetName);
        await source.DragToAsync(target);
    }

    // --- Выпадающие списки и скролл ---
    protected async Task DoSelectOption(ILocator locator, string name, string label)
    {
        Log.Information("[{Component}] Выбор значения '{Label}' в списке '{Name}'", _componentName, label, name);
        await locator.SelectOptionAsync(new SelectOptionValue { Label = label });
    }

    protected async Task DoScrollToElement(ILocator locator, string name)
    {
        Log.Information("[{Component}] Прокрутка к элементу '{Name}'", _componentName, name);
        await locator.ScrollIntoViewIfNeededAsync();
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
}