using FarvaterWeb.Base;
using FarvaterWeb.Extensions;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Users
{
    public record DepartmentDetails(string Name, string Code);
    [Collection("AllureCollection")]
    public class DepartmentCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);
        private UsersPage Users => new UsersPage(Page, Log, _test);

        public DepartmentCreationTests(ITestOutputHelper output) : base(output)
        {
            

        }

        [Fact(DisplayName = "Проверка успешного создания нового подразделения")]
        public async Task SouldCreateNewDepartment()
        {
            Log.Information("--- Запуск сценария: Создание нового подразделения---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Пользователи", "users");

            await Users.ClickTab("Подразделения");

            await Users.ClickCreateDepartmentButton();

            string postfix = DataPostfixExtensions.GetUniquePostfix();

            var newDepartmentDetails = new DepartmentDetails(
               Name: $"Тестовое подразделение {postfix}",
               Code: $"{postfix}"
               );

            await Users.FillDepartmentDetails(newDepartmentDetails);

            await Users.ClickAddButton();

            await Users.DeleteDepartment(newDepartmentDetails.Name);




        }

        



    }
}
