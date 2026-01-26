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

        public SmartLocator ButtonWithText(string text)
        {
            var locator = ModalDialogRoot.Locator("");

            return new SmartLocator(locator, text);
        }
    }
}
