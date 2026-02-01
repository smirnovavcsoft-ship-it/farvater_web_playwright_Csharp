using Microsoft.Playwright;
using FarvaterWeb.Extensions;

namespace FarvaterWeb.Components
{
    public class DropdownComponent
    {
        private readonly IPage _page;
        private readonly string _componentName;

        public DropdownComponent(IPage page, string componentName)
        {
            _page = page;
            _componentName = componentName;
        }

        public SmartLocator WithLabel(string label)
        {
            var container = _page.Locator("div[class*='_Flex_']")
        .Filter(new()
        {
            Has = _page.Locator("span, div, label")      
                       .GetByText(label, new() { Exact = false })       
        })
        .Last;

            var interactiveElement = container.Locator("[data-testid='dropdown_list']").First;

            return new SmartLocator(interactiveElement, label, "Dropdown", _componentName, _page);
        }

        public SmartLocator WithText(string text)
        {
            var locator = _page.Locator("[data-testid='dropdown_list']")
                .Filter(new() { Has = _page.Locator($"div:text-is('{text}'), input[placeholder='{text}']") });

            return new SmartLocator(locator, text, "выпадающий список", _componentName, _page);
        }

        public SmartLocator WithLocator(ILocator customLocator, string friendlyName) =>
            new SmartLocator(customLocator, friendlyName, "особенный список", _componentName, _page);
    }

}


