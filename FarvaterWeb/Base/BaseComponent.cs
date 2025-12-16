using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace FarvaterWeb.Base
{
    public abstract class BaseComponent
    {
        protected readonly IPage Page;
        protected readonly ILocator? Root;
        protected readonly ILogger Log;
        private readonly string _componentName;

        protected BaseComponent(IPage page, ILogger logger, ILocator? root = null)
        {
            Page = page;
            Log = logger;
            Root = root;
            _componentName = GetType().Name;
        }

        // --- Автоматизированные методы взаимодействия ---

        protected async Task DoClick(ILocator locator, string elementName)
        {
            Log.Information($"[{_componentName}] Нажатие на '{elementName}'");
            // Playwright сам подождет кликабельности (auto-waiting)
            await locator.ClickAsync();
        }

        protected async Task DoFill(ILocator locator, string elementName, string text, bool maskText = false)
        {
            string logValue = maskText ? "****" : text;
            Log.Information($"[{_componentName}] Ввод '{logValue}' в поле '{elementName}'");
            await locator.FillAsync(text);
        }

        protected async Task<string> DoGetText(ILocator locator, string elementName)
        {
            var text = await locator.TextContentAsync() ?? string.Empty;
            Log.Information($"[{_componentName}] Получен текст из '{elementName}': '{text}'");
            return text;
        }

        // Метод, который вернет либо Root, либо саму страницу для поиска
        protected ILocator GetContext() => Root ?? Page.Locator("html");
    }

    // Здесь могут быть общие методы, например, для ожидания загрузки или проверки видимости
    /*protected async Task WaitForVisible(ILocator locator)
    {
        await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }*/

}
