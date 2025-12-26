using FarvaterWeb.Base;
using FarvaterWeb.Pages;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Users;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Users
{
    public class PositionCreationTests : BaseTest
    {

        //var signInPage = new SignInPage(Page, Log, _test);
        //var dashboardPage = new DashboardPage(Page, Log, _test);

        private readonly DashboardPage _dashboardPage;
        private readonly UsersPage _usersPage;

        public PositionCreationTests(ITestOutputHelper output) : base(output)
        {
            _dashboardPage = new DashboardPage(Page, Log, _test);
            _usersPage = new UsersPage(Page, Log, _test);

        }
        [Fact]
        public async Task SouldCreateNewPosition()
        {
            await LoginAsAdmin();
            await _dashboardPage.OpenSection("Пользователи", "users");
        }
    }

}
