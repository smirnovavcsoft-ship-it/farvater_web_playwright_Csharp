using Microsoft.Playwright;
using FarvaterWeb.Extensions;

namespace FarvaterWeb.Components
{
    public class DropdownComponent
    {
        private readonly IPage _page;
        private readonly ILocator _row;
        private readonly string _label;
        private readonly string _pageName;

        public DropdownComponent(IPage page, string label, string pageName)
        {
            _page = page;
            _label = label;
            _pageName = pageName;

            // Вместо поиска по любому div._Flex_so6fa_1, мы ищем контейнер DropDown,
            // который содержит лейбл именно с этим текстом. 
            // .First помогает отсечь родительские элементы, если они всё же попадут в выборку.
            _row = _page.Locator("div._DropDownSelect_16801_1")
                        .Filter(new() { Has = _page.Locator($"._label_16801_163:text-is('{_label}')") })
                        .Locator("xpath=.."); // Поднимаемся ровно на один уровень к родителю (_Flex)
        }

        // Основное поле выбора
        public SmartLocator Control =>
            new SmartLocator(
                _row.Locator("._controlWrapper_16801_8"),
                _label,
                "выпадающий список",
                _pageName,
                _page);

        // Кнопка "+" рядом с полем
        public SmartLocator CreateButton =>
            new SmartLocator(
                _row.Locator("button[data-signature='button-wrapper']"),
                $"Создать '{_label}'",
                "кнопка '+'",
                _pageName,
                _page);
    }
}