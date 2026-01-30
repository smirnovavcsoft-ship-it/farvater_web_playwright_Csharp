using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.Pages.Common
{
    public class SideMenuPage : BasePage
    {
        // Оставляем только те локаторы, которые уникальны (меню пользователя)

        
        private ILocator UserDropdown => Page.Locator("//div[starts-with(@class, '_user_name_')]");
        private ILocator LogoutItem => Page.Locator("//div[@data-signature='dropdown-menu-item' and .//div[text()='Выйти']]");

        public SideMenuComponent Menu => new SideMenuComponent(Page, Log, _test, pageName);

        public SideMenuPage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test)
        {
            
        }

        // ОДИН метод для всех переходов по меню
        //public async Task OpenSection(string name, string urlPart)
        //{
        //    // Находим ссылку в меню по ее текстовому названию
        //    var menuButton = Page.GetByRole(AriaRole.Link, new() { Name = name });

        //    // Кликаем через базовый метод (с логированием в отчет)
        //    await DoClick(menuButton, $"Переход в раздел '{name}'");

        //    // Ждем, пока URL изменится на нужный
        //    await Page.WaitForURLAsync($"**/{urlPart}**");

        //    Log.Information($"[SideMenu] Переход в раздел {name} выполнен");
            
        //}

        public async Task OpenSection(string name, string urlPart)
        {
            await Menu.ClickItem(name);
        }



public async Task Logout()
        {
            await DoClick(UserDropdown, "Меню пользователя");
            await DoClick(LogoutItem, "Пункт 'Выйти'");
            await Page.WaitForURLAsync("**/signin**");
        }
    }
}