using FarvaterWeb.Base;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Users
{
    public record UsesDetails(
        string Firstname,
        string Name,
        string Middlename,
        string IDnumber,
        string UserLogin,
        string Phone,
        string Email        
        );
    public class UserCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private UsersPage Users => new UsersPage(Page, Log, _test);

        public UserCreationTests(ITestOutputHelper output) : base(output) { } 

        [Fact(DisplayName ="Проверка успешного создания пользователя")]

        public async async ShouldCreateUser ()
        {
            Log.Information("--- Запуск сценария: Создание нового пользователя---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Пользователи", "users");

            // Клик по вкладке "Пользователи"

            await Users.ClickTab("Пользователи");

            // Клик по кнопке "Добавить пользователя"



            // Заполнение полей нового пользователя

            // Выбор подразделения

            // Выбор должности

            // Клик по чек-боксу "Является руководителем
        }
    }
}
