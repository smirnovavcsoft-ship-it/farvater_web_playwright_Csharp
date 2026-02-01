using Microsoft.Playwright;

namespace FarvaterWeb.Extensions
{
    public static partial class AssertionExtensions
    {
        public static async Task AssertVisibleAsync(this SmartLocator smart, int timeout = 10000)
        {
            await smart.Page.Do($"[Проверка] '{smart.Name}' (тип: {smart.Type}) должен быть виден", async () =>
            {
                await Assertions.Expect(smart.Locator).ToBeVisibleAsync(new() { Timeout = timeout });
            });
        }

        public static async Task AssertTextAsync(this SmartLocator smart, string expectedText)
        {
            await smart.Page.Do($"[Проверка] Текст в '{smart.Name}' должен быть '{expectedText}'", async () =>
            {
                await Assertions.Expect(smart.Locator).ToHaveTextAsync(expectedText);
            });
        }

        public static async Task AssertEnabledAsync(this SmartLocator smart)
        {
            await smart.Page.Do($"[Проверка] Поле '{smart.Name}' должно быть доступно для ввода", async () =>
            {
                await Assertions.Expect(smart.Locator).ToBeEnabledAsync();
            });
        }
    }
}
