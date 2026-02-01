using FarvaterWeb.Base;
using Microsoft.Playwright;

namespace FarvaterWeb.Extensions
{
    public static class PwExtensions
    {
        public static async Task Do(this IPage page, string stepName, Func<Task> action)
        {
            Serilog.Log.Information(stepName);

            await AllureService.Step(stepName, action);
        }

        
        public static async Task SafeClickAsync(this SmartLocator smart)
        {
            string stepName = $"[{smart.ComponentName}] Клик по элементу: {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                await smart.Locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await smart.Locator.ClickAsync();
            });
        }

        public static async Task ClearAndFillAsync(this SmartLocator smart, string value)
        {
            string stepName = $"[{smart.ComponentName}] Клик по элементу: {smart.Type} '{smart.Name}'";
            await smart.Page.Do(stepName, async () =>
            {
                await smart.Locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await smart.Locator.ClickAsync(new() { Force = true });
                await smart.Locator.ClearAsync();
                await smart.Locator.PressSequentiallyAsync(value, new() { Delay = 50 });
            });
            
        }

        public static async Task PressEnterAsync(this ILocator locator)
        {
            await locator.PressAsync("Enter");
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

        public static async Task SetCheckedSafeAsync(this ILocator locator, bool state)
        {
            if (state)
                await locator.CheckAsync();
            else
                await locator.UncheckAsync();

            await Assertions.Expect(locator).ToBeCheckedAsync(new() { Checked = state });
        }

        public static async Task SelectOptionByTextAsync(this ILocator locator, string text)
        {
            await locator.SelectOptionAsync(new SelectOptionValue { Label = text });
        }

        public static async Task ShouldBeVisibleAsync(this ILocator locator, string message = "Элемент не отображается")
        {
            try
            {
                await Assertions.Expect(locator).ToBeVisibleAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{message}. Подробности: {ex.Message}");
            }
        }        
        

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
                var checkIcon = smart.Locator.Locator("svg._icon_on_oaaw3_66");

                bool isCurrentlyChecked = await checkIcon.IsVisibleAsync();

                if (isCurrentlyChecked != shouldBeChecked)
                {
                    await smart.Locator.ClickAsync();

                    if (shouldBeChecked)
                        await Assertions.Expect(checkIcon).ToBeVisibleAsync();
                    else
                        await Assertions.Expect(checkIcon).ToBeHiddenAsync();
                }
            });
        }
    }


}
