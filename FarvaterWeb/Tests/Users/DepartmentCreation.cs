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

            // Клик по вкладке "Подразделения"

            await Users.ClickTab("Подразделения");

            // Клик по кнопке "Создать подразделение"

            await Users.ClickCreateDepartmentButton();

            // Ввод наименования и кода подразделения
            string postfix = DataPostfixExtensions.GetUniquePostfix();

            var newDepartmentDetails = new DepartmentDetails(
               Name: $"Тестовое подразделение {postfix}",
               Code: $"{postfix}"
               );

            await Users.FillDepartmentDetails(newDepartmentDetails);

            // Клик по кнопке "Добавить"

            await Users.ClickAddButton();

            // Удаление созданного подразделения

            await Users.DeleteDepartment(newDepartmentDetails.Name);




        }

        



    }
}
