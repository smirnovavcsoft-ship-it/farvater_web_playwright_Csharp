using Microsoft.Playwright;
using FarvaterWeb.Extensions;

namespace FarvaterWeb.Components
{
    public class DropdownComponent
    {
        private readonly IPage _page;
        private readonly string _pageName;

        public DropdownComponent(IPage page, string pageName)
        {
            _page = page;
            _pageName = pageName;
        }

        // Универсальная логика нахождения "кликабельной" части
        private ILocator GetControl(ILocator container) =>
            container.Locator("input, [class*='control'], [class*='ValueContainer']").Last;

        // МЕТОД 1: По заголовку (DropdownWithLabel)
        public SmartLocator WithLabel(string label) =>
            new SmartLocator(
                GetControl(_page.Locator("div[class*='_DropDownSelect_']")
                    .Filter(new() { Has = _page.Locator($"[class*='_label_']:text-is('{label}')") })
                    .Locator("xpath=..").Last),
                label, "Выпадающий список", _pageName, _page);

        // МЕТОД 2: По тексту/placeholder внутри (DropdownWithText)
        public SmartLocator WithText(string text) =>
            new SmartLocator(
                GetControl(_page.Locator("div[class*='_Flex_']")
                    .Filter(new() { Has = _page.Locator($"input[placeholder='{text}'], div:text-is('{text}')") }).Last),
                text, "Выпадающий список", _pageName, _page);

        // МЕТОД 3: По селектору (DropdownWithSelector)
        public SmartLocator WithSelector(string selector, string friendlyName) =>
            new SmartLocator(
                GetControl(_page.Locator(selector).Last),
                friendlyName, "Выпадающий список", _pageName, _page);

        //

        public SmartLocator CreateButton(SmartLocator parent) =>
        new SmartLocator(
            parent.Locator.Locator("button[data-signature='button-wrapper']"),
            $"Создать в '{parent.Name}'",
            "кнопка '+'",
            _pageName,
            _page);
    }

}


