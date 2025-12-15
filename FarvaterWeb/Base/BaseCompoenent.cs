using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarvaterWeb.Base
{
    public abstract class BaseComponent
    {
        // protected, чтобы был доступен в наследниках
        protected readonly IPage Page;

        public BaseComponent(IPage page)
        {
            Page = page;
        }

        // Здесь могут быть общие методы, например, для ожидания загрузки или проверки видимости
        protected async Task WaitForVisible(ILocator locator)
        {
            await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }
    }
}
