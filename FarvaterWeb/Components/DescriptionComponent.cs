using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FarvaterWeb.Components
{
    public class DescriptionComponent
    {
        private readonly IPage _page;
        private readonly string _componentName;
        private readonly ILocator _root;

        public DescriptionComponent(IPage page, string componentName, ILocator root = null)
        {
            _page = page;
            _root = root ?? page.Locator("html");
            _componentName = componentName;
        }

        // Поиск по внешнему заголовку (над полем или слева)
        public SmartLocator WithLabel(string label)
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

        public SmartLocator WithPlacholder(string label)
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
        

        public SmartLocator WithLocator(ILocator customLocator, string friendlyName) =>
            new SmartLocator(customLocator, friendlyName, "Поле ввода", _componentName, _page);
    }
}

