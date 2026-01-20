using FarvaterWeb.Extensions;
using Microsoft.Playwright;

namespace FarvaterWeb.Components
{
    public class CheckboxComponent
    {
        private readonly IPage _page;
        private readonly string _label;
        private readonly string _pageName;
        private readonly ILocator _container;

        public CheckboxComponent(IPage page, string label, string pageName)
        {
            _page = page;
            _label = label;
            _pageName = pageName;

            // Весь контейнер
            _container = _page.Locator("div._label_container_oaaw3_5")
                .Filter(new() { Has = _page.Locator("p", new() { HasText = _label }) });
        }

        // Возвращаем SmartLocator, указывающий на кликабельную область
        public SmartLocator Control =>
            new SmartLocator(_container.Locator("div._checkbox_out_oaaw3_23"), _label, "чекбокс", _pageName, _page);

        // Удобный метод для цепочки
        public async Task SetAsync(bool state) => await Control.SetCustomCheckboxAsync(state);
    }
}