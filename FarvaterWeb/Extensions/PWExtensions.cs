using FarvaterWeb.Base;
using Microsoft.Playwright;

namespace FarvaterWeb.Extensions
{
    public static class PwExtensions
    {
        public static async Task Do(this IPage page, string stepName, Func<Task> action)
        {
            // Используем статический доступ к Serilog
            Serilog.Log.Information(stepName);

            // Для ExtentReports обычно тоже есть статический доступ или контекст
            // Если _test у тебя в BaseComponent, можно передавать его или сделать доступным
            // _test?.Info(stepName); 

            // Самое главное — Allure!
            await AllureService.Step(stepName, action);
        }

        /*public static async Task SafeClickAsync(this ILocator locator, string? label = null)
        {
            // Пытаемся автоматически определить имя, если оно не передано
            string name = label ?? "элемент";

            // Вызываем расширение Do у страницы, к которой относится локатор
            await locator.Page.Do($"Нажатие на '{name}'", async () =>
            {
                await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await locator.ClickAsync();
            });
        }*/

        public static async Task SafeClickAsync(this SmartLocator smart)
        {
            // Весь процесс логируется как одно действие
            await smart.Page.Do($"Клик по элементу: {smart.Type} '{smart.Name}'", async () =>
            {
                // Сначала неявное ожидание, чтобы шаг не упал мгновенно
                await smart.Locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                // Само действие
                await smart.Locator.ClickAsync();
            });
        }

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

        /*public static async Task ClickWithWaitAsync(this ILocator locator, float timeout = 5000)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            await locator.ClickAsync();
        }*/

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
            try
            {
                // Стандартный ассерт без лишних аргументов
                await Assertions.Expect(locator).ToBeVisibleAsync();
            }
            catch (Exception ex)
            {
                // Перехватываем и добавляем твой текст к ошибке
                throw new Exception($"{message}. Подробности: {ex.Message}");
            }
        }

        
        // Выбор по индексу (номеру)
        /*public static async Task SelectByIndexAndVerifyAsync(this ILocator dropdown, int index)
        {
            await dropdown.ClickAsync();

            // Ищем опции по всей странице (часто выпадающие списки рендерятся в конце body)
            var options = dropdown.Page.GetByRole(AriaRole.Option);
            await options.First.WaitForAsync(); // Ждем, пока список подгрузится

            var targetOption = options.Nth(index);
            string optionText = (await targetOption.InnerTextAsync()).Trim();

            await targetOption.ClickAsync();

            // Проверяем, что текст в контроле изменился на выбранный
            await Assertions.Expect(dropdown).ToContainTextAsync(optionText);
        }*/

        // Выбор по тексту (на будущее)
        public static async Task SelectByTextAndVerifyAsync(this ILocator dropdown, string text)
        {
            await dropdown.ClickAsync();

            var targetOption = dropdown.Page.GetByRole(AriaRole.Option, new() { Name = text, Exact = true });
            await targetOption.ClickAsync();

            await Assertions.Expect(dropdown).ToContainTextAsync(text);
        }
    }


}
