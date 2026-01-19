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
        /*public static async Task SelectByIndexAndVerifyAsync(this SmartLocator smart, int index)
        {
            // Формируем имя шага в твоем стиле
            string stepName = $"[{smart.ComponentName}] Выбор пункта №{index + 1} в элементе: {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                // 1. Кликаем по самому контролу (используем smart.Locator)
                await smart.Locator.ClickAsync();

                // 2. Находим опции на странице
                var options = smart.Page.GetByRole(AriaRole.Option);
                await options.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var targetOption = options.Nth(index);
                string optionText = (await targetOption.InnerTextAsync()).Trim();

                // 3. Выбираем
                await targetOption.ClickAsync();

                // 4. Проверяем
                await Assertions.Expect(smart.Locator).ToContainTextAsync(optionText);
            });
        }*/

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

        /*public static async Task SelectByTextAndVerifyAsync(this SmartLocator smart, string text)
        {
            string stepName = $"[{smart.ComponentName}] Выбор пункта '{text}' в {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                await smart.Locator.ClickAsync(new() { Force = true });

                // Ищем конкретный пункт по тексту
                var targetOption = smart.Page
                    .Locator("[data-testid='dropdown_list-options'] [data-signature='dropdown_list-item']")
                    .GetByText(text, new() { Exact = true });

                await targetOption.ClickAsync();

                await Assertions.Expect(smart.Locator).ToContainTextAsync(text);
            });
        }*/

        /*public static async Task SelectByIndexAndVerifyAsync(this SmartLocator smart, int index)
        {
            string stepName = $"[{smart.ComponentName}] Выбор пункта №{index + 1} в {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                // 1. Кликаем по самому дропдауну, чтобы открыть меню
                await smart.Locator.ClickAsync(new() { Force = true });

                // 2. Ищем контейнер с опциями, который только что появился
                var optionsContainer = smart.Page.Locator("[data-testid='dropdown_list-options']");

                // Ждем, чтобы контейнер стал видимым (защита от анимации)
                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                // 3. Находим все пункты внутри этого контейнера по их сигнатуре
                var options = optionsContainer.Locator("[data-signature='dropdown_list-item']");

                // Берем нужный по индексу
                var targetOption = options.Nth(index);

                // Сохраняем текст (например, "Руководство"), чтобы потом проверить, что он выбрался
                string optionText = (await targetOption.InnerTextAsync()).Trim();

                // 4. Кликаем по пункту
                await targetOption.ClickAsync();

                // 5. Проверка: текст в контроле (smart.Locator) должен измениться на выбранный
                // В твоем случае текст попадает в ._header_text_... внутри контрола
                await Assertions.Expect(smart.Locator).ToContainTextAsync(optionText);
            });
        }*/

        public static async Task SelectByIndexAndVerifyAsync(this SmartLocator smart, int index)
        {
            string stepName = $"[{smart.ComponentName}] Выбор пункта №{index + 1} в {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                // 1. Кликаем по дропдауну
                await smart.Locator.ClickAsync(new() { Force = true });

                // 2. Решаем проблему Strict Mode: 
                // Если на странице два видимых списка, берем самый последний (.Last), 
                // так как новый список обычно рендерится в конце DOM-дерева.
                var optionsContainer = smart.Page
                    .Locator("[data-testid='dropdown_list-options']:visible")
                    .Last;

                // Ждем, чтобы он точно был готов
                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                // 3. Работаем внутри найденного контейнера
                var options = optionsContainer.Locator("[data-signature='dropdown_list-item']");

                // Проверяем, что элементов в списке достаточно
                int count = await options.CountAsync();
                if (index >= count)
                    throw new Exception($"Не удается выбрать индекс {index}, в списке всего {count} элементов.");

                var targetOption = options.Nth(index);
                string optionText = (await targetOption.InnerTextAsync()).Trim();

                // 4. Кликаем по пункту
                await targetOption.ClickAsync();

                // 5. Ждем, пока список исчезнет, чтобы он не мешал следующему шагу
                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                // 6. Проверка текста
                await Assertions.Expect(smart.Locator).ToContainTextAsync(optionText);
            });
        }

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
