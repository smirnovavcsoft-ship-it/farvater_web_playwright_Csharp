using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.Components
{
    public class TableComponent
    {
        private readonly IPage _page;

        public TableComponent(IPage page)
        {
            _page = page;
        }

        /// <summary>
        /// Универсальный метод удаления строки по тексту (имени)
        /// </summary>
        /// <param name="rowText">Текст, по которому ищем строку (название компании)</param>
        /// <param name="deleteIconSelector">Селектор иконки корзины</param>
        public async Task DeleteRowByText(string rowText, string deleteIconSelector)
        {
            Log.Information("Попытка удаления строки с текстом: {Text}", rowText);

            // 1. Находим нужную строку таблицы
            var row = _page.Locator("tr").Filter(new() { HasText = rowText }).First;

            // 2. Находим иконку удаления ВНУТРИ этой строки
            var deleteButton = row.Locator(deleteIconSelector);

            // 3. Скроллим и кликаем
            await row.ScrollIntoViewIfNeededAsync();
            await deleteButton.ClickAsync();

            Log.Information("Клик по корзине выполнен для: {Text}", rowText);
        }
    }
}
