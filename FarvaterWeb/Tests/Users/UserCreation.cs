using FarvaterWeb.Base;
using FarvaterWeb.Extensions;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using System.Xml.Linq;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Users
{
    public record UserDetails(
        string Lastname,
        string Name,
        string Middlename,
        string IDnumber,
        string UserLogin,
        string Phone,
        string Email        
        );

    [Collection("AllureCollection")]
    public class UserCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private UsersPage Users => new UsersPage(Page, Log, _test);

        public UserCreationTests(ITestOutputHelper output) : base(output) { } 

        [Fact(DisplayName ="Проверка успешного создания пользователя")]

        public async Task ShouldCreateUser ()
        {
            Log.Information("--- Запуск сценария: Создание нового пользователя---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Пользователи", "users");

            // Клик по вкладке "Пользователи"

            await Users.ClickUsersTab();
            // Клик по кнопке "Добавить пользователя"

            await Users.ClickAddUser();

            // Заполнение полей нового пользователя

            // 1. Генерируем один постфикс для всего юзера
            string postfix = DataExtensions.GetUniquePostfix();

            // 2. Создаем запись, используя интерполяцию строк $""
            var userDetails = new UserDetails(
                Lastname: "Тестеренко",
                Name: "Анатолий",
                Middlename: "Владимирович",
                IDnumber: $"{postfix}",                
                UserLogin: $"testerenko{postfix}",         
                Phone: $"+7(812)123-{postfix.Substring(0, 2)}-{postfix.Substring(2, 2)}", 
                Email: $"testirenko{postfix}@company.ru"         
            );

            await Users.FillUserDetails(userDetails);

            // Выбор первого подразделения из списка

            await Users.SelectFirstDepartment();

            // Создание и выбор подразделения

            await Users.CreateDepartment();

            var newDepartmentDetails = new DepartmentDetails(
               Name: $"Тестовое подразделение {postfix}",
               Code: $"{postfix}"
               );

            await Users.FillDepartmentDetails(newDepartmentDetails);

            // Выбор первой должности из списка

            await Users.SelectFirstPosition();

            // Создание и выбор должности

            await Users.CreatePosition();

            await Users.FillPositionName(newDepartmentDetails.Name);

            // Клик по чек-боксу "Является руководителем" и "Имеет право подписи"

            await Users.IsABoss();

            await Users.HaveARightToSign();

            // Клик по кнопке "Создать"

            await Users.CreateButton();

            // Открытие карточки созданного пользователя

            await Users.VerifyUserCreated(userDetails.Email);

            // Откритие карточки созданного пользователя



            // Клик по кнопке "Уволить"



            // Выбор сотрудника, которому передаются задачи

            //Нажатие кнопки "Уволить"

            // Переход в раздел "Пользователи

            // Переход во вкладку "Неакивные пользователи" и проверка наличия удаленного пользователя.




        }
    }
}
