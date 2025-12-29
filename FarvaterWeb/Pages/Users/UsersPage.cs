using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Tests.Counterparty;
using Microsoft.Playwright;
using Serilog;
using System.Xml.Linq;

namespace FarvaterWeb.Pages.Users
{
    public class UsersPage : BasePage
    {
        public UsersPage(IPage page, ILogger logger, ExtentTest extentTest) : base(page, logger, extentTest) { }

        public async Task ClickPositionsTab()
        {
            await DoClickByText("Должности");
        }

        public async Task ClickCreatePositionButton()
        {
            await DoClickByText("Создать должность");
        }

        public async Task DeletePosition(string name)
        {
            // Указываем специфичный селектор корзины для этой страницы
            // Точка в начале означает поиск по классу
            const string deleteIcon = ".menuItemDelete";

            // Вызываем метод компонента
            await Table.DeleteRowByText(name, deleteIcon);

            // Подтверждаем удаление в модальном окне (это уже логика страницы или ModalComponent)
            await Page.GetByRole(AriaRole.Button, new() { Name = "Да" }).ClickAsync();


        }

        public async Task FillPositionName(string name)
        {
            await DoFillByLabel("Наименование", name);
        }


    }
}
