using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;
using AventStack.ExtentReports;

namespace FarvaterWeb.Pages.Common
{
    public class SignInPage : BasePage
    {
        private readonly string _path = "https://farvater.mcad.dev/farvater/signin";

        private ILocator UsernameInput => Page.Locator("//input[@data-signature='auth-user-name-input']");
        private ILocator PasswordInput => Page.Locator("//input[@data-signature='password-input']");
        private ILocator LoginButton => Page.Locator("//button[.//span[text()='Войти']]");

        public SignInPage(IPage page, Serilog.ILogger logger, ExtentTest test) : base(page, logger, test) { }

        public async Task NavigateAsync() => await GoToUrl(_path, "signin");

        public async Task LoginAsync(string username, string password)
        {
            await DoFill(UsernameInput, "Имя пользователя", username);
            await DoFill(PasswordInput, "Пароль", password);

            await DoClick(LoginButton, "Кнопка 'Войти'");

            await Page.WaitForURLAsync("**/dashboard**");
            Log.Information("[SignInPage] Вход выполнен успешно, открыт Dashboard");
        }
    }
}