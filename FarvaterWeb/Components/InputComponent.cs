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

        // Поиск по внешнему заголовку (над полем или слева)
        public SmartLocator DescriptionField(string label)
        {
            // 1. Ищем контейнер (тот самый div class="_shortDescription_...")
            // Мы ищем его по наличию текста заголовка внутри.
            // Exact = false позволяет игнорировать звездочку и лишние пробелы.
            var container = _page.Locator("div")
                .Filter(new()
                {
                    Has = _page.GetByText(label, new() { Exact = false })
                })
                .Last;

            // 2. Ищем поле ввода внутри этого контейнера.
            // В твоей верстке это div с role="textbox" и contenteditable="true".
            // Мы перечисляем варианты через запятую (селектор "или").
            var interactiveElement = container
                .Locator("div[contenteditable='true'], textarea, input:not([type='file'])")
                .First;

            return new SmartLocator(interactiveElement, label, "Поле ввода", _componentName, _page);
        }

        // Поиск по подсказке (placeholder) внутри самого поля
        public SmartLocator WithText(string text)
        {
            // Здесь нам не нужен Filter, так как placeholder — это атрибут самого input.
            // Используем *= чтобы искать по частичному совпадению текста.
            var container = _page.Locator($"input[placeholder*='{text}'], textarea[placeholder*='{text}']").First;

            return new SmartLocator(container, text, "Поле ввода", _componentName, _page);
        }

        public SmartLocator WithLabel(string label)
        {
            // 1. Находим контейнер всего поля по тексту заголовка внутри него
            var container = _page.Locator("div[data-testid='input-text-wrapper']")
                .Filter(new()
                {
                    Has = _page.Locator("[data-testid='input-text-title']",
                          new() { HasText = label })
                });

            // 2. Внутри этого конкретного контейнера берем само поле ввода
            // Мы ищем и textarea, и input на случай, если другие поля будут обычными инпутами
            var input = container.Locator("textarea, input");

            return new SmartLocator(input, label, "Поле ввода", _componentName, _page);
        }

        public SmartLocator WithLocator(ILocator customLocator, string friendlyName) =>
            new SmartLocator(customLocator, friendlyName, "Поле ввода", _componentName, _page);
    }
}
