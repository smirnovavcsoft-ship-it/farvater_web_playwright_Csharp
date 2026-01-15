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
        public static async Task SelectByIndexAndVerifyAsync(this ILocator dropdown, int index)
        {
            // 1. Кликаем по списку, чтобы он открылся
            await dropdown.ClickAsync();

            // 2. Находим все доступные опции. 
            // В большинстве современных веб-приложений пункты списка имеют роль 'option'
            var options = dropdown.Page.GetByRole(AriaRole.Option);

            // Ждем, чтобы хотя бы одна опция появилась в DOM
            await options.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

            // Проверяем, не выходит ли индекс за границы (опционально, Playwright сам упадет на Nth(index))
            var targetOption = options.Nth(index);

            // Получаем текст выбранного элемента для последующей проверки
            string optionText = (await targetOption.InnerTextAsync()).Trim();

            // 3. Выбираем элемент
            await targetOption.ClickAsync();

            // 4. Проверка: текст в контроле должен содержать текст выбранной опции
            // Это гарантирует, что выбор действительно применился в UI
            await Assertions.Expect(dropdown).ToContainTextAsync(optionText);
        }

        /// <summary>
        /// Выбирает элемент из выпадающего списка по точному текстовому совпадению
        /// </summary>
        /// <param name="dropdown">Локатор самого поля списка</param>
        /// <param name="text">Текст, который нужно выбрать</param>
        public static async Task SelectByTextAndVerifyAsync(this ILocator dropdown, string text)
        {
            // 1. Кликаем по списку
            await dropdown.ClickAsync();

            // 2. Ищем опцию с конкретным текстом (Exact = true важен, чтобы не выбрать похожие)
            var targetOption = dropdown.Page.GetByRole(AriaRole.Option, new() { Name = text, Exact = true });

            // Ждем появления и кликаем
            await targetOption.ClickAsync();

            // 3. Проверка результата
            await Assertions.Expect(dropdown).ToContainTextAsync(text);
        }
    }
}
