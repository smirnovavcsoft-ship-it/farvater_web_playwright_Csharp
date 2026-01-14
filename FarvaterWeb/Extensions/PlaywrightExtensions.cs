using Microsoft.Playwright;

namespace FarvaterWeb.Extensions
{
    public static class PlaywrightExtensions
    {      

        // --- Группа 1: Ввод данных ---

        public static async Task ClearAndFillAsync(this ILocator locator, string value)
        {
            await locator.ClearAsync();
            await locator.FillAsync(value);
        }

        public static async Task PressEnterAsync(this ILocator locator)
        {
            await locator.PressAsync("Enter");
        }

        // --- Группа 2: Кнопки и клики ---

        public static async Task ClickWithWaitAsync(this ILocator locator, float timeout = 5000)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            await locator.ClickAsync();
        }

        public static async Task DoubleClickWithWaitAsync(this ILocator locator, float timeout = 5000)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            await locator.DblClickAsync();
        }

        // --- Группа 3: Выбор (Selection) ---

        public static async Task SetCheckedSafeAsync(this ILocator locator, bool state)
        {
            if (state)
                await locator.CheckAsync();
            else
                await locator.UncheckAsync();

            // Встроенная проверка результата
            await Assertions.Expect(locator).ToBeCheckedAsync(new() { Checked = state });
        }

        public static async Task SelectOptionByTextAsync(this ILocator locator, string text)
        {
            // Стандартный метод выбора в <select>
            await locator.SelectOptionAsync(new SelectOptionValue { Label = text });
        }

        // --- Группа 4: Ожидание и состояние ---

        public static async Task ShouldBeVisibleAsync(this ILocator locator, string message = "Элемент не отображается")
        {
            // Кастомное сообщение об ошибке помогает быстрее понять, что упало
            await Assertions.Expect(locator).ToBeVisibleAsync(new() { Message = message });
        }
    }


}
