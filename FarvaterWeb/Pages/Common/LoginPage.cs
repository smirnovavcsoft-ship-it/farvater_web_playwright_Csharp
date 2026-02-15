using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Configuration;
using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.Pages.Common
{
    public class LoginPage : BasePage
    {
        public LoginPage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test)
        {

        }

        protected async Task LoginAs(string login, string password)
        {
            Log.Information($"[Setup] Начало авторизации под {login}");

            await Page.GotoAsync(ConfigurationReader.BaseUrl);

            await Page.GetByPlaceholder("Пользователь").FillAsync(login);
            await Page.GetByPlaceholder("Пароль").FillAsync(password);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Войти" }).ClickAsync();

            await Page.WaitForURLAsync("**/dashboard");

            _test.Info("Авторизация выполнена успешно");
            Log.Information("[Setup] Авторизация успешна");
        }
    }
}
