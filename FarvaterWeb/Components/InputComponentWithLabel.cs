using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarvaterWeb.Components
{
    public class InputComponentWithLabel
    {
        private readonly IPage _page;
        private readonly string _labelName;

        // Конструктор принимает IPage и текст заголовка (Label)
        public InputComponentWithLabel(IPage page, string labelName)
        {
            _page = page;
            _labelName = labelName;
        }

        // Приватное свойство-локатор, которое находит конкретный элемент
        private ILocator FieldContainer => _page.Locator("[data-signature='form-field']")
            // Фильтруем родительский контейнер по наличию нужного текста заголовка
            .Filter(new() { HasText = _labelName });

        // Публичное свойство-локатор для самого поля ввода внутри найденного контейнера
        public ILocator Input => FieldContainer.Locator("input");

        // Метод для инкапсуляции действия
        public async Task FillAsync(string value)
        {
            await Input.FillAsync(value);
        }
    }
}
