using FarvaterWeb.Base;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Xunit.Abstractions;
using FarvaterWeb.TestData;
using FarvaterWeb.Data;
using FarvaterWeb.ApiServices;
using FarvaterWeb.Extensions;
using FarvaterWeb.Generators;

namespace FarvaterWeb.Tests.Users
{
    //public record DepartmentDetails(string Name, string Code);
    //[Collection("AllureCollection")]
    public class DepartmentCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);
        private UsersPage Users => new UsersPage(Page, Log, _test);

        public DepartmentCreationTests(ITestOutputHelper output) : base(output)
        {
            

        }

        //[Fact(DisplayName = "Проверка успешного создания нового подразделения")]
        [Theory(DisplayName = "Проверка успешного создания нового подразделения")]
        [MemberData(nameof(DepartmentTestData.GetUniversalDepartmentCases), MemberType = typeof(DepartmentTestData))]
        public async Task ShouldCreateNewDepartment(UserModel actor, DepartmentDetails department)
        {
            Log.Information("--- Запуск сценария: Создание нового подразделения---");
            await LoginAs(actor.Login!);
            await SideMenu.OpenSection("Пользователи", "users");

            // Клик по вкладке "Подразделения"

            await Users.ClickTab("Подразделения");

            // Клик по кнопке "Создать подразделение"

            await Users.ClickCreateDepartmentButton();

            // Ввод наименования и кода подразделения
            

            await Users.FillDepartmentDetails(department);

            // Клик по кнопке "Добавить"

            await Users.ClickAddButton();

            // Удаление созданного подразделения

            await Users.DeleteDepartment(department.Name);




        }

        



    }
}
