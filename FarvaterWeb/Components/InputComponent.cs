using Microsoft.Playwright;
using FarvaterWeb.Extensions;
using System.ComponentModel.Design.Serialization;

namespace FarvaterWeb.Components
{
    public class InputComponent
    {
        private readonly IPage _page;
        private readonly string _componentName;
        private readonly ILocator _root;

        public InputComponent(IPage page, string componentName, ILocator root = null)
        {
            _page = page;
            _root = root ?? page.Locator("html");
            _componentName = componentName;
        }

        public SmartLocator DescriptionField(string label)
        {
            var container = _page.Locator("div")
                .Filter(new()
                {
                    Has = _page.GetByText(label, new() { Exact = false })
                })
                .Last;

            var interactiveElement = container
                .Locator("div[contenteditable='true'], textarea, input:not([type='file'])")
                .First;

            return new SmartLocator(interactiveElement, label, "Поле ввода", _componentName, _page);
        }

        public SmartLocator WithText(string text)
        {
            var container = _page.Locator($"input[placeholder*='{text}'], textarea[placeholder*='{text}']").First;

            return new SmartLocator(container, text, "Поле ввода", _componentName, _page);
        }

        public SmartLocator WithLabel(string label)
        {
            var container = _page.Locator("div[data-testid='input-text-wrapper']")
                .Filter(new()
                {
                    Has = _page.Locator("[data-testid='input-text-title']",
                          new() { HasText = label })
                });

            var input = container.Locator("textarea, input");

            return new SmartLocator(input, label, "Поле ввода", _componentName, _page);
        }

        public SmartLocator WithLocator(ILocator customLocator, string friendlyName) =>
            new SmartLocator(customLocator, friendlyName, "Поле ввода", _componentName, _page);
    }
}
