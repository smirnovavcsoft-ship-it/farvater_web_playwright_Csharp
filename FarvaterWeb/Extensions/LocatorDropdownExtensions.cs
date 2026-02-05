using Microsoft.Playwright;

namespace FarvaterWeb.Extensions
{
    /// <summary>
    /// Расширения для работы с выпадающими списками (Dropdown/Select)
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Выбирает элемент из выпадающего списка по его порядковому номеру (индексу)
        /// </summary>
        /// <param name="dropdown">Локатор самого поля списка</param>
        /// <param name="index">Индекс элемента (0, 1, 2...)</param>
        
        /// <summary>
        /// Выбирает элемент из выпадающего списка по точному текстовому совпадению
        /// </summary>
        /// <param name="dropdown">Локатор самого поля списка</param>
        /// <param name="text">Текст, который нужно выбрать</param>
        /*public static async Task SelectByTextAndVerifyAsync(this ILocator dropdown, string text)
        {
            // 1. Кликаем по списку
            await dropdown.ClickAsync();

            // 2. Ищем опцию с конкретным текстом (Exact = true важен, чтобы не выбрать похожие)
            var targetOption = dropdown.Page.GetByRole(AriaRole.Option, new() { Name = text, Exact = true });

            // Ждем появления и кликаем
            await targetOption.ClickAsync();

            // 3. Проверка результата
            await Assertions.Expect(dropdown).ToContainTextAsync(text);
        }*/

        public static async Task SelectByTextAndVerifyAsync(this SmartLocator smart, string text)
        {
            string stepName = $"[{smart.ComponentName}] Выбор пункта '{text}' в {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                await smart.Locator.ClickAsync(new() { Force = true });

                // --- УМНАЯ ПАУЗА №1 ---
                // Ждем, пока контейнер списка станет видимым. 
                // Если списка нет в DOM, поиск по тексту может вернуть старые/чужие элементы.
                var listContainer = smart.Page.Locator("[data-testid='dropdown_list-options']").First;
                await listContainer.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                // Ищем конкретный пункт по тексту
                var targetOption = smart.Page
                    .Locator("[data-signature='dropdown_list-item']")
                    .GetByText(text, new() { Exact = true }).Last;

                //.Locator("[data-testid='dropdown_list-options'] [data-signature='dropdown_list-item']")

                await targetOption.ClickAsync();

                await Assertions.Expect(smart.Locator).ToContainTextAsync(text);
            });
        }

       

       

        public static async Task SelectByIndexAndVerifyAsync(
    this SmartLocator smart,
    int index,
    bool isMultiSelect = false,
    string? customVerifyLocator = null)
        {
            string stepName = $"[{smart.ComponentName}] Выбор пункта №{index + 1} в {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                // 1. Открываем дропдаун (Force помогает при нестабильности)
                await smart.Locator.ClickAsync(new() { Force = true });

                // 2. Находим контейнер списка (Last решает проблему нескольких списков в DOM)
                var optionsContainer = smart.Page
                    .Locator("[data-testid='dropdown_list-options']:visible")
                    .Last;

                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                // 3. Получаем текст и кликаем по пункту
                var targetOption = optionsContainer.Locator("[data-signature='dropdown_list-item']").Nth(index);
                string optionText = (await targetOption.InnerTextAsync()).Trim();

                await targetOption.ClickAsync(new() { Force = true });

                // 4. Логика для мультиселектора
                if (isMultiSelect)
                {
                    await smart.Page.Keyboard.PressAsync("Escape");
                    // Ждем закрытия, чтобы не "споткнуться" в следующем шаге
                    await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
                }

                // 5. Проверка результата
                var verifyLocator = !string.IsNullOrEmpty(customVerifyLocator)
                    ? smart.Page.Locator(customVerifyLocator)
                        : isMultiSelect
                            ? smart.Page.Locator("[data-signature='mutliselect-list']") //локатор выбранного пункта, который появляется ниже выпадающего списка
                            : smart.Locator;

                await Assertions.Expect(verifyLocator).ToContainTextAsync(optionText);
            });
        }
    
    }
}
