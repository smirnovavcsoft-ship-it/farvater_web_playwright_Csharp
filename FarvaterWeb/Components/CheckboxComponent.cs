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

            _container = _page.Locator("[data-signature='checkbox-wrapper']")
                          .Filter(new() { HasText = _label });
        }


        public SmartLocator Control =>
            new SmartLocator(_container, _label, "чекбокс", _pageName, _page);

        public async Task SetAsync(bool state)
        {
            await Control.SetCustomCheckboxAsync(state);
        }
    }
}