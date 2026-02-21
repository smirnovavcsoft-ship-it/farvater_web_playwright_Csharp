using FarvaterWeb.Base;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Xunit.Abstractions;
using FarvaterWeb.TestData;
using FarvaterWeb.Data;
using FarvaterWeb.ApiServices;

namespace FarvaterWeb.Tests.Users
{
    //[Collection("AllureCollection")]
    public class PositionCreationTests : BaseTest
    {        
        //private readonly SideMenuPage _sideMenuPage;
        //private readonly UsersPage _usersPage;
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);
        private UsersPage Users => new UsersPage(Page, Log, _test);

        private UserApiService UserApi => new UserApiService(ApiRequest);



        public PositionCreationTests(ITestOutputHelper output) : base(output)
        {
            //_sideMenuPage = new SideMenuPage(Page, Log, _test);
            //_usersPage = new UsersPage(Page, Log, _test);

        }
        //[AllureOwner("AlexanderSmirnov")]
        //[AllureSuite("Пользователи")] // Это будет главная папка в отчете
        // [AllureSubSuite("Должности")]
        // [Fact(DisplayName = "Проверка успешного создания новой должности")]
        [Theory(DisplayName = "Проверка успешного создания новой должности")]
        [MemberData(nameof(PositionTestData.GetUniversalPositionCases), MemberType = typeof(PositionTestData))]

        public async Task ShouldCreateNewPosition(UserModel actor, PositionDetails position)
        {
            //var allureResultsPath = AllureLifecycle.Instance.ResultsDirectory;
           // Log.Information($"Allure results path: {allureResultsPath}");

            try
            {
                Log.Information("--- Запуск сценария: Создание новой должности---");
                await LoginAs(actor.Login!);
                await SideMenu.OpenSection("Пользователи", "users");

                // Клик по вкладке "Должности"

                await Users.ClickTab("Должности");

                // Клик по кнопке "Создать должность"

                await Users.ClickCreatePositionButton();

                // Ввод наименования должности

                
                await Users.FillPositionName(position.Name);

                // Клик по кнопке "Отмена" и проверка создания должности

                await Users.CancelAndVerify(position.Name);


                // Клик по кнопке "Создать должность"

                await Users.ClickCreatePositionButton();

                // Ввод наименования должности

                await Users.FillPositionName(position.Name);

                // Клик по кнопке "Добавить"

                await Users.ClickAddButton();

                // Проверка наличия созданно должности на странице

                await Users.VerifyPositionCreated(position.Name);

                // Удаление должности

                await Users.DeletePosition(position.Name);



                Log.Information("Тест успешно завершен.");

                MarkTestAsPassed();
            }
            catch (Exception ex)
            {
                // Передаем текст ошибки прямо в отчет перед тем, как «уронить» тест для xUnit
                _test.Fail($"<b>Критическая ошибка:</b> {ex.Message}<br>StackTrace: {ex.StackTrace}");
                throw; // Пробрасываем ошибку дальше, чтобы xUnit пометил тест красным
            }

        }
    }

}
