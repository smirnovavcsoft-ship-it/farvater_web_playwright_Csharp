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

        /*public SmartLocator WithLabel(string label)
        {
            var locator = _page.Locator("div._DropDownSelect_16801_1")
                .Filter(new() { Has = _page.Locator($"._label_16801_163:text-is('{label}')") });

            return new SmartLocator(locator, label, "выпадающий список", _componentName, _page);
        }*/

        public SmartLocator WithLabel(string label)
        {
            // Ищем общий Flex-контейнер, в котором лежит и лейбл, и кнопка
            var container = _page.Locator("div[class*='_Flex_']")
                .Filter(new() { Has = _page.Locator($"._label_16801_163:text-is('{label}')") })
                .Last; // .Last нужен, если на странице есть вложенные флексы

            return new SmartLocator(container, label, "Dropdown", _componentName, _page);
        }

        public SmartLocator WithText(string text)
        {
            var locator = _page.Locator("div._DropDownSelect_16801_1")
                .Filter(new() { Has = _page.Locator($"div:text-is('{text}'), input[placeholder='{text}']") });

            return new SmartLocator(locator, text, "выпадающий список", _componentName, _page);
        }

        public SmartLocator WithLocator(ILocator customLocator, string friendlyName) =>
            new SmartLocator(customLocator, friendlyName, "особенный список", _componentName, _page);
    }

}


