using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;
using AventStack.ExtentReports;

namespace FarvaterWeb.Components
{
    // Наследуем BaseUI — это дает нам Do() и AutoScreenshot()
    public class TableComponent : BaseComponent
    {
        public TableComponent(IPage page, ILogger logger, ExtentTest test)
            : base(page, logger, test, "Table")
        {
        }

        /// <summary>
        /// Универсальный метод: найти строку по тексту и нажать в ней кнопку/иконку
        /// </summary>
        public async Task ClickActionInRow(string rowText, string actionSelector, string actionName)
        {
            await Do($"[Таблица] Действие '{actionName}' для строки '{rowText}'", async () =>
            {
                // 1. Ищем строку
                var row = Page.Locator("tr").Filter(new() { HasText = rowText }).First;

                // 2. Ищем элемент внутри строки (например, корзину или карандаш)
                var actionElement = row.Locator(actionSelector);

                // 3. Скроллим и кликаем
                await actionElement.ScrollIntoViewIfNeededAsync();
                await actionElement.ClickAsync(new() { Force = true });

                // 4. Скриншот результата
                await AutoScreenshot($"{actionName}_{rowText.Replace(" ", "_")}");
            });
        }

        /// <summary>
        /// Упрощенный метод специально для удаления
        /// </summary>
        public async Task DeleteRow(string rowText, string buttonText)
        {
            // Используем селектор корзины по умолчанию, который мы нашли ранее
            await ClickActionInRow(rowText, "div[class*='menuItemDelete']", "Удаление");

            await DoClickByText(buttonText);
        }


        // Метод для удаления конкретного локатора (внутренний). Понадобится, когда буду делать проверку наличия элемента перед созданием
        private async Task DeleteRow(ILocator rowLocator)
        {
            // Находим кнопку удаления внутри этой конкретной строки
            var deleteBtn = rowLocator.Locator("button.delete-action"); // замените на ваш селектор
            await deleteBtn.ClickAsync();

            // Подтверждение в модальном окне (если оно есть)
            var confirmBtn = Page.Locator("button:has-text('Да')");
            if (await confirmBtn.IsVisibleAsync()) await confirmBtn.ClickAsync();
        }
    }
}