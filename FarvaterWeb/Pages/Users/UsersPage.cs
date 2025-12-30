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

        public async Task ClickTab(string tabName)
        {
            Log.Information("[UsersPage] Переход на вкладку: {TabName}", tabName);

            // Используем локатор по тексту
            await Page.GetByText(tabName, new() { Exact = true }).ClickAsync();

            // Опционально: подождать, пока вкладка станет активной 
            // (в вашем HTML активная вкладка помечается наличием внутри div класса _switchOutline_)
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

        public async Task CancelAndVerify(string PositionName)
        {
            
            await CancelAction.ExecuteAndVerify(PositionName);
        }




    }
}
