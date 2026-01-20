using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;
using AventStack.ExtentReports;
using FarvaterWeb.Extensions; // Важно для работы SmartLocator и SafeClickAsync

namespace FarvaterWeb.Components
{
    public class TableComponent : BaseComponent
    {
        public TableComponent(IPage page, ILogger logger, ExtentTest test)
            : base(page, logger, test, "Table")
        {
        }

        // --- СТАРЫЕ МЕТОДЫ (Оставляем как есть) ---

        public async Task ClickActionInRow(string rowText, string actionSelector, string actionName)
        {
            await Do($"[Таблица] Действие '{actionName}' для строки '{rowText}'", async () =>
            {
                var row = Page.Locator("tr").Filter(new() { HasText = rowText }).First;
                var actionElement = row.Locator(actionSelector);
                await actionElement.ScrollIntoViewIfNeededAsync();
                await actionElement.ClickAsync(new() { Force = true });
                await AutoScreenshot($"{actionName}_{rowText.Replace(" ", "_")}");
            });
        }

        public async Task DeleteRow(string rowText, string buttonText)
        {
            await ClickActionInRow(rowText, "div[class*='menuItemDelete']", "Удаление");
            await DoClickByText(buttonText);
        }

        private async Task DeleteRow(ILocator rowLocator)
        {
            var deleteBtn = rowLocator.Locator("button.delete-action");
            await deleteBtn.ClickAsync();
            var confirmBtn = Page.Locator("button:has-text('Да')");
            if (await confirmBtn.IsVisibleAsync()) await confirmBtn.ClickAsync();
        }

        // --- НОВЫЕ МЕТОДЫ (Для гибкости и цепочек) ---

        /// <summary>
        /// Возвращает SmartLocator для всей строки. 
        /// Позволяет делать Grid.Row("Имя").SafeClickAsync()
        /// </summary>
        public SmartLocator Row(string rowText)
        {
            var locator = Page.Locator("tr").Filter(new() { HasText = rowText }).First;
            return new SmartLocator(locator, rowText, "Строка таблицы", _componentName, Page);
        }

        /// <summary>
        /// Позволяет найти любой элемент внутри строки и вернуть его как SmartLocator
        /// </summary>
        public SmartLocator ControlInRow(string rowText, string selector, string elementName)
        {
            var locator = Page.Locator("tr")
                .Filter(new() { HasText = rowText })
                .First
                .Locator(selector);

            return new SmartLocator(locator, $"{elementName} в строке '{rowText}'", "Элемент таблицы", _componentName, Page);
        }

        // 1. Создаем готовый метод для удаления (селектор спрятан здесь)
        public SmartLocator DeleteButton(string rowText) =>
            ControlInRow(rowText, "div[class*='menuItemDelete']", "Удалить");

        // 2. Создаем метод для редактирования (если селектор известен)
        public SmartLocator EditButton(string rowText) =>
            ControlInRow(rowText, "button.edit-action", "Редактировать");
    }
}