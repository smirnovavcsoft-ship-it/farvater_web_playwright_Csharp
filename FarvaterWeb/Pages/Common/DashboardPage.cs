using Microsoft.Playwright;
using FarvaterWeb.Base;

namespace FarvaterWeb.Pages.Common
{
    public class DashboardPage : BasePage
    {
        // Локаторы как свойства
        private ILocator ProjectsTab => Page.Locator("//a[contains(@href, 'projects')]");
        private ILocator CounterpartyTab => Page.Locator("//a[contains(@href, 'counterparty')]");
        private ILocator UserDropdown => Page.Locator("//div[starts-with(@class, '_user_name_')]");
        private ILocator LogoutItem => Page.Locator("//div[@data-signature='dropdown-menu-item' and .//div[text()='Выйти']]");

        public DashboardPage(IPage page, Serilog.ILogger logger) : base(page, logger) { }

        public async Task NavigateToCounterparty()
        {
            await DoClick(CounterpartyTab, "Вкладка 'Контрагенты'");
            await Page.WaitForURLAsync("**/counterparty**");
        }

        public async Task Logout()
        {
            await DoClick(UserDropdown, "Меню пользователя");
            // Наш базовый метод уже умеет ждать и логировать
            await DoClick(LogoutItem, "Пункт 'Выйти'");
            await Page.WaitForURLAsync("**/signin**");
        }
    }
}