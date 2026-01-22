using Microsoft.Playwright;
using FarvaterWeb.Extensions;

namespace FarvaterWeb.Components
{
    public class InputComponent
    {
        private readonly IPage _page;
        private readonly string _componentName;

        public InputComponent(IPage page, string componentName)
        {
            _page = page;
            _componentName = componentName;
        }

        // Поиск по внешнему заголовку (над полем или слева)
        public SmartLocator WithLabel(string label)
        {
            // 1. Ищем контейнер (div), в котором есть текст нашей метки.
            // 2. Внутри этого контейнера находим само поле (input или textarea).
            var container = _page.Locator("div")
                .Filter(new() { Has = _page.Locator($"._label_16801_163:text-is('{label}')") })
                .Locator("input, textarea").First;

            return new SmartLocator(container, label, "Поле ввода", _componentName, _page);
        }

        // Поиск по подсказке (placeholder) внутри самого поля
        public SmartLocator WithText(string text)
        {
            // Здесь нам не нужен Filter, так как placeholder — это атрибут самого input.
            // Используем *= чтобы искать по частичному совпадению текста.
            var container = _page.Locator($"input[placeholder*='{text}'], textarea[placeholder*='{text}']").First;

            return new SmartLocator(container, text, "Поле ввода", _componentName, _page);
        }

        public SmartLocator WithLocator(ILocator customLocator, string friendlyName) =>
            new SmartLocator(customLocator, friendlyName, "Поле ввода", _componentName, _page);
    }
}
