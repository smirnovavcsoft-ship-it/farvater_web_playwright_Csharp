using FarvaterWeb.Base;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Xunit.Abstractions;
using FarvaterWeb.TestData;
using FarvaterWeb.Data;
using FarvaterWeb.ApiServices;

namespace FarvaterWeb.Tests.Users
{
    public record PermissionDetails(
    bool IsAdmin = true,
    bool IsGip = true,
    bool IsArchive = true,
    bool IsContracts = true,
    bool IsOrd = true
    );

    //[Collection("AllureCollection")]
    public class GroupCreationTests : BaseTest
    {
        private UserApiService UserApi => new UserApiService(ApiRequest);
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);
        private UsersPage Users => new UsersPage(Page, Log, _test);

        public GroupCreationTests(ITestOutputHelper output) : base(output) { }

        //[Fact(DisplayName = "Проверка успешного создания новой группы")]
        [Theory(DisplayName = "Проверка успешного создания новой группы")]
        [MemberData(nameof(GroupTestData.GetUniversalGroupCases), MemberType = typeof(GroupTestData))]

        public async Task ShouldCreateNewGroup(UserModel actor, UserModel newUser, GroupDetails group)
        {
            string? userHandle = null;


            // Создание пользователя через API

            try
            {
                Log.Information("--- Запуск сценария: Создание новой группы---");
                await LoginAs(actor.Login!);
                await SideMenu.OpenSection("Пользователи", "users");

                // Клик по вкладке "Группы"

                await Users.ClickTab("Группы");

                // Клик по кнопке "Создать группу"

                await Users.ClickCreateGroupButton();

                // Ввод наименования группы

                // string groupName = "Тестировщики";

                // await Users.FillGroupName(groupName);


                // Заполнение Details

                /* var permissionDetails = new PermissionDetails(
                         IsAdmin: true,
                         IsGip: true,
                         IsArchive: true,
                         IsContracts: true,
                         IsOrd: true                    
                  );*/



                await Users.FillGroupDetails(group, newUser);

                // Выбор ответственных

               // await Users.SelectFirstResponsiblePerson();

                // Клик по кнопке "Создать"

                await Users.ClickCreateButton();

                // Удаление созданной группы

                await Users.DeleteGroup(group.GroupName);
            }
            finally
            {
                // Если GUID был получен — удаляем


                if (!string.IsNullOrEmpty(userHandle))
                {
                    await UserApi.DismissUserAsync(userHandle);
                }
            }
        }
    }
}
