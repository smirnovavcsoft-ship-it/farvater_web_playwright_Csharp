using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.Components
{
    public class CancelComponent
    {
        private readonly IPage _page;

        public CancelComponent(IPage page)
        {
            _page = page;
        }

        public async Task CancelAndVerify(string unexpectedText)
        {
            Log.Information("[CancelComponent] Проверка отмены для данных: {Text}", unexpectedText);

            // 1. Ищем кнопку по жестко заданному имени "Отмена"
            var cancelButton = _page.GetByRole(AriaRole.Button, new() { Name = "Отмена" });

            // Кликаем по ней
            await cancelButton.ClickAsync();

            // 2. Сразу проверяем, что кнопка "Отмена" исчезла (значит, форма закрылась)
            await Assertions.Expect(cancelButton).ToBeHiddenAsync(new() { Timeout = 5000 });

            // 3. Проверяем, что в таблице (строках tr) нет нашего текста
            var row = _page.Locator("tr").Filter(new() { HasText = unexpectedText });
            await Assertions.Expect(row).ToBeHiddenAsync(new() { Timeout = 3000 });

            Log.Information("[CancelComponent] Проверка успешна: форма закрыта, '{Text}' не создан", unexpectedText);
        }
    }
}