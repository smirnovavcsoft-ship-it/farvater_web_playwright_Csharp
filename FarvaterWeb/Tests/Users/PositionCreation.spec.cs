using Allure.Xunit.Attributes.Steps;
using FarvaterWeb.Base;
using FarvaterWeb.Pages;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Users
{
    public class PositionCreationTests : BaseTest
    {        
        //private readonly SideMenuPage _sideMenuPage;
        //private readonly UsersPage _usersPage;
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);
        private UsersPage Users => new UsersPage(Page, Log, _test);



        public PositionCreationTests(ITestOutputHelper output) : base(output)
        {
            //_sideMenuPage = new SideMenuPage(Page, Log, _test);
            //_usersPage = new UsersPage(Page, Log, _test);

        }
        [Fact (Skip = "Пока этот тест не нужен")]
        public async Task SouldCreateNewPosition()
        {
            await LoginAsAdmin();
            await SideMenu.OpenSection("Пользователи", "users");

            // Клик по вкладке "Должности"

            await Users.ClickTab("Должности");

            // Клик по кнопке "Создать должность"

            await Users.ClickCreatePositionButton();

            // Ввод наименования должности

            string positionName = "Тестовая должность (удалить)";

            await Users.FillPositionName(positionName);

            // Клик по кнопке "Отмена"

            await Users.CancelAndVerify(positionName);

            // Проверка наличия созданной должности на странице



            // Клик по кнопке "Создать должность"

            // Ввод наименования должности

            // Клик по кнопке "Добавить"

            // Проверка наличия созданно должности на странице

            // Удаление должности

        }
    }

}
