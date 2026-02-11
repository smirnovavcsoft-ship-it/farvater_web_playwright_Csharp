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

        /*public SmartLocator WithLabel(string label)
        {
            // Ищем общий Flex-контейнер, в котором лежит и лейбл, и кнопка
            var container = _page.Locator("div[class*='_Flex_']")
                .Filter(new() { Has = _page.Locator($"._label_16801_163:text-is('{label}')") })
                .Last; // .Last нужен, если на странице есть вложенные флексы

            return new SmartLocator(container, label, "Dropdown", _componentName, _page);
        }*/

        public SmartLocator WithLabel(string label)
        {
            // 1. Ищем контейнер по стабильной сигнатуре (она была в твоем HTML)
            // Если сигнатуры нет у контейнера, ищем просто div, который содержит наш текст
            var container = _page.Locator("div[class*='_Flex_']")
        .Filter(new()
        {
            Has = _page.Locator("span, div, label") // Ищем в любом текстовом элементе
                       .GetByText(label, new() { Exact = false }) // Найдет и со звездочкой, и без
        })
        .Last;

            // 2. Внутри этого контейнера ищем то, на что можно нажать.
            // Я заметил в твоем логе 'dropdown_list-wrapper' — давай добавим и его.
            var interactiveElement = container.Locator("[data-testid='dropdown_list'], [data-testid='user-selector-input'], [data-testid='toggle-list']").First;

            return new SmartLocator(interactiveElement, label, "Dropdown", _componentName, _page);
        }

        public SmartLocator WithInputAndLabel(string label)
        {
            // 1. Ищем контейнер по стабильной сигнатуре (она была в твоем HTML)
            // Если сигнатуры нет у контейнера, ищем просто div, который содержит наш текст
            var container = _page.Locator("div[class*='_Flex_']")
        .Filter(new()
        {
            Has = _page.Locator("span, div, label") // Ищем в любом текстовом элементе
                       .GetByText(label, new() { Exact = false }) // Найдет и со звездочкой, и без
        })
        .Last;

            // 2. Внутри этого контейнера ищем то, на что можно нажать.
            // Я заметил в твоем логе 'dropdown_list-wrapper' — давай добавим и его.
            var interactiveElement = container.Locator("[data-testid='toggle-list']").First;

            return new SmartLocator(interactiveElement, label, "Dropdown", _componentName, _page);
        }

        public SmartLocator WithText(string text)
        {
            //var locator = _page.Locator("div._DropDownSelect_16801_1")
            var locator = _page.Locator("[data-testid='dropdown_list']")
                .Filter(new() { Has = _page.Locator($"div:text-is('{text}'), input[placeholder='{text}']") });

            return new SmartLocator(locator, text, "выпадающий список", _componentName, _page);
        }

        public SmartLocator WithLocator(ILocator customLocator, string friendlyName) =>
            new SmartLocator(customLocator, friendlyName, "особенный список", _componentName, _page);
    }

}


