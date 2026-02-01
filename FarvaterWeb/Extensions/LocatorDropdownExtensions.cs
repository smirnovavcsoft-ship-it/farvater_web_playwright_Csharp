using Microsoft.Playwright;

namespace FarvaterWeb.Extensions
{
    public static partial class Extensions
    {
        public static async Task SelectByIndexAndVerifyAsync(
    this SmartLocator smart,
    int index,
    bool isMultiSelect = false,
    string? customVerifyLocator = null)
        {
            string stepName = $"[{smart.ComponentName}] Выбор пункта №{index + 1} в {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                await smart.Locator.ClickAsync(new() { Force = true });

                var optionsContainer = smart.Page
                    .Locator("[data-testid='dropdown_list-options']:visible")
                    .Last;

                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var targetOption = optionsContainer.Locator("[data-signature='dropdown_list-item']").Nth(index);
                string optionText = (await targetOption.InnerTextAsync()).Trim();

                await targetOption.ClickAsync(new() { Force = true });

                if (isMultiSelect)
                {
                    await smart.Page.Keyboard.PressAsync("Escape");
                    await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
                }

                var verifyLocator = !string.IsNullOrEmpty(customVerifyLocator)
                    ? smart.Page.Locator(customVerifyLocator)
                        : isMultiSelect
                            ? smart.Page.Locator("[data-signature='mutliselect-list']")        
                            : smart.Locator;

                await Assertions.Expect(verifyLocator).ToContainTextAsync(optionText);
            });
        }
    
    }
}
