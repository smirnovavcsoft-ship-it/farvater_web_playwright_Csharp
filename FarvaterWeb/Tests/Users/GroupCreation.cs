using FarvaterWeb.Base;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Users
{
    public record PermissionDetails(
    bool IsAdmin = true,
    bool IsGip = true,
    bool IsArchive = true,
    bool IsContracts = true,
    bool IsOrd = true
    );

    [Collection("AllureCollection")]
    public class GroupCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);
        private UsersPage Users => new UsersPage(Page, Log, _test);

        public GroupCreationTests(ITestOutputHelper output) : base(output) { }

        [Fact(DisplayName = "Проверка успешного создания новой группы")]

        public async Task SouldCreateNewGroup()
        {
            Log.Information("--- Запуск сценария: Создание новой группы---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Пользователи", "users");

            // Клик по вкладке "Группы"

            await Users.ClickTab("Группы");

            // Клик по кнопке "Создать группу"

            await Users.ClickCreateGroupButton();

            // Ввод наименования группы

            string groupName = "Тестировщики";

            await Users.FillGroupName(groupName);


            // Клик на чек-боксы

            var permissionDetails = new PermissionDetails(
                    IsAdmin: true,
                    IsGip: true,
                    IsArchive: true,
                    IsContracts: true,
                    IsOrd: true                    
             );

            await Users.SetPermissions(permissionDetails);

            // Выбор ответственных

            await Users.SelectFirstResponsiblePerson();

            // Клик по кнопке "Создать"

            await Users.ClickCreateButton();

            // Удаление созданной группы

            await Users.DeleteGroup(groupName);
        }
    }
}
