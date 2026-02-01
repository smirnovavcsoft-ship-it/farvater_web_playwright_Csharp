using AventStack.ExtentReports;
using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;
using FarvaterWeb.Extensions;

namespace FarvaterWeb.Components
{
    public class ModalDialogComponent : BaseComponent
    {
        private ILocator ModalDialogRoot => Page.Locator("[data-signature='modal-dialog-content']");

        public ModalDialogComponent(IPage page, ILogger logger, ExtentTest test)
            : base(page, logger, test) { }

        public SmartLocator Button(string text)
        {
            var locator = ModalDialogRoot
                .Locator("[data-signature='button-wrapper']")
                .Filter(new() { HasText = text });

            return new SmartLocator(locator, text, "Кнопка", _componentName, Page);
        }

        public ILocator GetInput(string labelText)
        {
            return ModalDialogRoot
                .Locator("[data-signature='input-field-wrapper']")
                .Filter(new() { Has = Page.Locator("[data-signature='input-text-title']", new() { HasText = labelText }) })
                .Locator("[data-signature='input-field-input']");
        }

        public async Task ClickButtonAsync(string text)
        {
            await Do($"Нажатие кнопки '{text}'", async () =>
            {
                await Button(text).Locator.ClickAsync();
                if (text == "Добавить" || text == "Отмена")
                {
                    await ModalDialogRoot.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
                }
            });
        }
    }
}
