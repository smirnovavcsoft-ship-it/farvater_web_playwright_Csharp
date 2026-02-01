using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using FarvaterWeb.Base;
using FarvaterWeb.Extensions;
using FarvaterWeb.Pages;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Users
{
    [Collection("AllureCollection")]
    public class PositionCreationTests : BaseTest
    {        
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);
        private UsersPage Users => new UsersPage(Page, Log, _test);



        public PositionCreationTests(ITestOutputHelper output) : base(output)
        {
        }
        [AllureOwner("AlexanderSmirnov")]
        [AllureSuite("Пользователи")]       
        [AllureSubSuite("Должности")]
        [Fact(DisplayName = "Проверка успешного создания новой должности")]
        public async Task SouldCreateNewPosition()
        {
            var allureResultsPath = AllureLifecycle.Instance.ResultsDirectory;
            Log.Information($"Allure results path: {allureResultsPath}");

            try
            {
                Log.Information("--- Запуск сценария: Создание новой должности---");
                await LoginAsAdmin();
                await SideMenu.OpenSection("Пользователи", "users");

                await Users.ClickTab("Должности");

                await Users.ClickCreatePositionButton();

                string postfix = DataPostfixExtensions.GetUniquePostfix();

                string positionName = $"Тестовая должность {postfix}";

                await Users.FillPositionName(positionName);

                await Users.CancelAndVerify(positionName);


                await Users.ClickCreatePositionButton();

                await Users.FillPositionName(positionName);

                await Users.ClickAddButton();

                await Users.VerifyPositionCreated(positionName);

                await Users.DeletePosition(positionName);



                Log.Information("Тест успешно завершен.");

                MarkTestAsPassed();
            }
            catch (Exception ex)
            {
                _test.Fail($"<b>Критическая ошибка:</b> {ex.Message}<br>StackTrace: {ex.StackTrace}");
                throw;         
            }

        }
    }

}
