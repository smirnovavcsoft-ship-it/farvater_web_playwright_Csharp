using Microsoft.Playwright;
using FarvaterWeb.Base;
using AventStack.ExtentReports;

namespace FarvaterWeb.Pages.Common
{
    public class SideMenuPage : BasePage
    {
        // Оставляем только те локаторы, которые уникальны (меню пользователя)

        private readonly IPage _page;
        private ILocator UserDropdown => Page.Locator("//div[starts-with(@class, '_user_name_')]");
        private ILocator LogoutItem => Page.Locator("//div[@data-signature='dropdown-menu-item' and .//div[text()='Выйти']]");

        public SideMenuPage(IPage page, Serilog.ILogger logger, ExtentTest test) : base(page, logger, test)
        {
            _page = page;
        }

        // ОДИН метод для всех переходов по меню
        public async Task OpenSection(string name, string urlPart)
        {
            // Находим ссылку в меню по ее текстовому названию
            var menuButton = Page.GetByRole(AriaRole.Link, new() { Name = name });

            // Кликаем через базовый метод (с логированием в отчет)
            await DoClick(menuButton, $"Переход в раздел '{name}'");

            // Ждем, пока URL изменится на нужный
            await Page.WaitForURLAsync($"**/{urlPart}**");

            Log.Information($"[SideMenu] Переход в раздел {name} выполнен");
        }

        public async Task Logout()
        {
            await DoClick(UserDropdown, "Меню пользователя");
            await DoClick(LogoutItem, "Пункт 'Выйти'");
            await Page.WaitForURLAsync("**/signin**");
        }
    }
}