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

        
        public static async Task SafeClickAsync(this SmartLocator smart)
        {
            // Формируем ту самую строку: [NewLegalPage] Клик по элементу: кнопка 'Создать'
            string stepName = $"[{smart.ComponentName}] Клик по элементу: {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                await smart.Locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await smart.Locator.ClickAsync();
            });
        }

        // --- Группа 1: Ввод данных ---

        public static async Task ClearAndFillAsync(this SmartLocator smart, string value)
        {
            string stepName = $"[{smart.ComponentName}] Клик по элементу: {smart.Type} '{smart.Name}'";
            await smart.Page.Do(stepName, async () =>
            {
                await smart.Locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await smart.Locator.ClickAsync(new() { Force = true });
                await smart.Locator.ClearAsync();
                //await smart.Locator.FillAsync(value);
                await smart.Locator.PressSequentiallyAsync(value, new() { Delay = 50 });
            });
            
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
        

        // Выбор по тексту (на будущее)
        public static async Task SelectByTextAndVerifyAsync(this ILocator dropdown, string text)
        {
            await dropdown.ClickAsync();

            var targetOption = dropdown.Page.GetByRole(AriaRole.Option, new() { Name = text, Exact = true });
            await targetOption.ClickAsync();

            await Assertions.Expect(dropdown).ToContainTextAsync(text);
        }

        public static async Task SetCustomCheckboxAsync(this SmartLocator smart, bool shouldBeChecked)
        {
            string stateText = shouldBeChecked ? "включить" : "выключить";
            string stepName = $"[{smart.ComponentName}] Попытка {stateText} {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                // Признак включенного чекбокса в твоей верстке — наличие иконки (svg)
                var checkIcon = smart.Locator.Locator("svg._icon_on_oaaw3_66");

                // Проверяем текущее состояние: видна ли галочка?
                bool isCurrentlyChecked = await checkIcon.IsVisibleAsync();

                // Если текущее состояние не совпадает с тем, которое мы хотим — кликаем
                if (isCurrentlyChecked != shouldBeChecked)
                {
                    await smart.Locator.ClickAsync();

                    // Проверка, что клик сработал (ждем изменения состояния)
                    if (shouldBeChecked)
                        await Assertions.Expect(checkIcon).ToBeVisibleAsync();
                    else
                        await Assertions.Expect(checkIcon).ToBeHiddenAsync();
                }
            });
        }
    }


}
