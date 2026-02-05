using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using System.ComponentModel.Design.Serialization;
using System.Text.RegularExpressions;

namespace FarvaterWeb.Components
{
    public class ButtonComponent
    {
        private readonly IPage _page;
        private readonly string _componentName;
        private readonly ILocator _root;

        public ButtonComponent(IPage page, string componentName, ILocator root = null)
        {
            _page = page;
            _root = root ?? page.Locator("html");
            _componentName = componentName;
        }

        public SmartLocator WithLabel(string label)
        {
            // Создаем регулярное выражение: 
            // ^ - начало строки
            // {Regex.Escape(label)} - твой текст (экранируем спецсимволы типа рубля)
            // .* - любые символы после (звездочка, пробелы, знаки валют)
            // $ - конец строки (чтобы не поймать "Стоимость доставки")
            var pattern = new Regex($"^{Regex.Escape(label)}.*$");

            var container = _page.Locator("div")
                // Фильтруем контейнеры, где заголовок СТРОГО соответствует паттерну
                .Filter(new()
                {
                    Has = _page.Locator("[data-signature='input-text-title'], [data-testid='input-text-title'], span")
                              .GetByText(pattern)
                })
                // Уточняем, что внутри этого дива должен быть инпут
                .Filter(new()
                {
                    Has = _page.Locator("input, textarea")
                })
                .Last; // Берем самый глубокий div, подходящий под условия

            var input = container.Locator("input, textarea").First;

            return new SmartLocator(input, label, "Поле ввода", _componentName, _page);
        }
    }
}
