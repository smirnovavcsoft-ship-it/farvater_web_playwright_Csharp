using FarvaterWeb.Base;
using FarvaterWeb.Extensions;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using System.Xml.Linq;
using Xunit.Abstractions;
using FarvaterWeb.Tests;

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

            await Users.ClickUsersTab();
            await Users.ClickAddUser();

           string postfix = DataPostfixExtensions.GetUniquePostfix();

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

            await Users.SelectFirstDepartment();

  

            var newDepartmentDetails = new DepartmentDetails(
               Name: $"Тестовое подразделение {postfix}",
               Code: $"{postfix}"
               );

         

            await Users.SelectFirstPosition();

            string positionName = "Тестовая должность {postfix}";

     

            await Users.IsABoss();

            await Users.HaveARightToSign();

            await Users.ClickCreateButton();

            await SideMenu.OpenSection("Пользователи", "users");
            await Users.VerifyUserCreated(userDetails.Email);

            await Users.OpenUserCard(userDetails.Email);

            await Users.ClickFireButton1();

            await Users.SelectReplacementEmployee();

            await Users.ClickFireButton2();

            await SideMenu.OpenSection("Пользователи", "users");

            await Users.ClickTab("Неактивные пользователи");
            await Users.VerifyUserCreated(userDetails.Email);

        }
    }
}
