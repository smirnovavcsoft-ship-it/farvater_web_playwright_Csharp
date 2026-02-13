using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using System.ComponentModel.Design.Serialization;
using System.Text.RegularExpressions;

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

        public SmartLocator DescriptionFieldWithPlacholder(string label)
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

        /*public SmartLocator WithLabel(string label)
        {
            // 1. Находим контейнер всего поля по тексту заголовка внутри него
            /*var container = _page.Locator("div[data-testid='input-text-wrapper']")
                .Filter(new()
                {
                    Has = _page.Locator("[data-testid='input-text-title']", 
                          new() { HasText = label })
                });*/

        /*var container = _page.Locator("div[data-testid='input-text-wrapper'], div[data-testid='inputField']")
            .Filter(new()
                    {
                        // Все варианты селекторов перечисляются внутри ОДНОЙ строки через запятую
                        Has = _page.Locator("[data-testid='input-text-title'], [data-testid='fieldLabel']")
                        .GetByText(label)
                        });

        // 2. Внутри этого конкретного контейнера берем само поле ввода
        // Мы ищем и textarea, и input на случай, если другие поля будут обычными инпутами
        var input = container.Locator("textarea, input");

        return new SmartLocator(input, label, "Поле ввода", _componentName, _page);
    }*/

        /*public SmartLocator WithLabel(string label)
        {
        // 1. Ищем контейнер (добавляем все возможные варианты через запятую)
        var container = _page.Locator("div[data-testid='input-text-wrapper'], div[data-testid='inputField'], div[data-signature='input-field-wrapper']")
            .Filter(new()
            {
                // 2. Ищем заголовок: и по testid, и по signature
                // Используем GetByText(label) без Exact, чтобы игнорировать <span>*</span>
                Has = _page.Locator("[data-testid='input-text-title'], [data-testid='fieldLabel'], [data-signature='input-text-title']")
                          .GetByText(label)
            });

        // 3. Достаем поле ввода (input или textarea)
        var input = container.Locator("textarea, input");

        return new SmartLocator(input, label, "Поле ввода", _componentName, _page);
        }*/

        /*public SmartLocator WithLabel(string label)
        {
            // 1. Ищем ЛЮБОЙ div, внутри которого есть наш текст (это будет либо обертка, либо родитель)
            var container = _page.Locator("div")
                .Filter(new() { HasText = label }) // Ищем текст "Стоимость, ₽" где-то внутри блока
                .Filter(new()
                {
                    // Уточняем, что в этом блоке обязательно должно быть поле ввода
                    Has = _page.Locator("[data-testid='inputField'], [data-signature='input-field-wrapper'], textarea, input")
                })
                .Last; // Берем ближайший к инпуту контейнер

            // 2. А теперь внутри этого найденного родителя забираем сам инпут
            var input = container.Locator("input, textarea").First;

            return new SmartLocator(input, label, "Поле ввода", _componentName, _page);
        }*/

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

        public SmartLocator WithLocator(ILocator customLocator, string friendlyName) =>
            new SmartLocator(customLocator, friendlyName, "Поле ввода", _componentName, _page);
    }
}
