using FarvaterWeb.Base;
using FarvaterWeb.Extensions;
//using FarvaterWeb.Base;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Xunit.Abstractions;
using FarvaterWeb.TestData;
using FarvaterWeb.Data;
using FarvaterWeb.ApiServices;

namespace FarvaterWeb.Tests.Users
{








    //[Collection("AllureCollection")]
    public class UserCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private UsersPage Users => new UsersPage(Page, Log, _test);

        private DepartmentApiService DepartmentApi => new DepartmentApiService(ApiRequest);

        private PositionApiService PositionApi => new PositionApiService(ApiRequest);

        public UserCreationTests(ITestOutputHelper output) : base(output) { }

        //[Fact(DisplayName ="Проверка успешного создания пользователя")]
        [Theory(DisplayName = "Проверка успешного создания пользователя")]
        [MemberData(nameof(UserTestData.GetUniversalUserCases), MemberType = typeof(UserTestData))]
        public async Task ShouldCreateUser(UserModel actor, DepartmentModel department, PositionModel position, UserDetails user)
        {
            string? departmentHandle = null;
            string? positionHandle = null;
            string? userHandle = null;

            try
            {
                departmentHandle = await DepartmentApi.PrepareDepartmentAsync(department.Name!, department.Code!);
                Log.Information("--- Запуск сценария: Создание нового пользователя---");
                await LoginAs(actor.Login!);
                await SideMenu.OpenSection("Пользователи", "users");

                // Клик по вкладке "Пользователи"

                await Users.ClickUsersTab();
                // Клик по кнопке "Добавить пользователя"

                await Users.ClickAddUser();

                // Заполнение полей нового пользователя

                //string postfix = DataPostfixExtensions.GetUniquePostfix();

                //  Данные создаваемого пользователя
                /*var userDetails = new UserDetails(
                    LastName: "Тестеренко",
                    FirstName: "Анатолий",
                    //Middlename: "Владимирович",
                    IDnumber: $"{postfix}",                
                    Login: $"testerenko{postfix}",         
                    Phone: $"+7(812)123-{postfix.Substring(0, 2)}-{postfix.Substring(2, 2)}", 
                    Email: $"testirenko{postfix}@company.ru"         
                );*/

                await Users.FillUserDetails(user);

                // Выбор первого подразделения из списка

                await Users.SelectDepartment(department.Name);

                // Создание и выбор подразделения



                /*var newDepartmentDetails = new DepartmentDetails(
                   Name: $"Тестовое подразделение {postfix}",
                   Code: $"{postfix}"
                   );*/



                //await Users.CreateDepartmentInUserCard(newDepartmentDetails);

                // Выбор первой должности из списка

                await Users.SelectPosition();

                // Создание и выбор должности
                // string positionName = "Тестовая должность {postfix}";

                //  await Users.CreatePositionInUserCard(positionName);



                // Клик по чек-боксу "Является руководителем" и "Имеет право подписи"

                await Users.IsABoss();

                await Users.HaveARightToSign();

                // Клик по кнопке "Создать"

                await Users.ClickCreateButton();

                // Открытие карточки созданного пользователя
                await SideMenu.OpenSection("Пользователи", "users");
                //  await Users.VerifyUserCreated(userDetails.Email);

                // Откритие карточки созданного пользователя

                //  await Users.OpenUserCard(userDetails.Email);

                // Клик по кнопке "Уволить"

                await Users.ClickFireButton1();

                // Выбор сотрудника, которому передаются задачи

                await Users.SelectReplacementEmployee();

                //Нажатие кнопки "Уволить"

                await Users.ClickFireButton2();

                // Переход в раздел "Пользователи

                await SideMenu.OpenSection("Пользователи", "users");

                // Переход во вкладку "Неакивные пользователи" и проверка наличия удаленного пользователя.

                await Users.ClickTab("Неактивные пользователи");
                //  await Users.VerifyUserCreated(userDetails.Email);

            }
            finally
            {
                if (userHandle != null)
                {
                    await UsersApi.DeleteUserAsync(userHandle);
                }
                if (departmentHandle != null)
                {
                    await DepartmentApi.DeleteDepartmentAsync(departmentHandle);
                }
                if (positionHandle != null)
                {
                    await PositionApi.DeletePositionAsync(positionHandle);
                }
            }

        }
    }
}
