using Microsoft.Playwright;
using FarvaterWeb.Pages.Common; // Предполагаем, что BasePage находится здесь
using System.Threading.Tasks;
using System.IO;
using Microsoft.Playwright.Core; // Может потребоваться для некоторых расширений
using System; // Для исключений
using FarvaterWeb.Components;


namespace FarvaterWeb.Pages.Common
{
    // Класс страницы входа
    public class SignInPage : BasePage
    {
        // Относительный путь к странице
        private readonly string _path = "https://farvater.mcad.dev/farvater/signin";

        // --- Локаторы Playwright (XPath) ---

        // В Playwright for .NET локаторы хранятся как string или ILocator. 
        // В данном случае лучше оставить их строками для простоты.
        private readonly string UsernameInput = "//input[@data-signature='auth-user-name-input']"; //input[@data-signature='auth-user-name-input']
        //private readonly string UsernameInput = "div[@class = '_row_239t9_51']/input[@data - signature = 'auth-user-name-input']";
        //private readonly string UsernameInput =
        //"xpath=//div[@class='_row_239t9_51']/input[@data-signature='auth-user-name-input']";

        private readonly string PasswordInput = "//input[@data-signature='password-input']";
        private const string LoginButtonLocator = "//button[.//span[text()='Войти']]";

        public SignInPage(IPage page, string baseUrl, string username, string password) : base(page, baseUrl, username, password)
        {
            var usernameField = new InputFieldComponent(page, "Имя пользователя");
            // Конструктор инициализирует родительский класс BasePage
            // и получает доступ к Page, Username, Password.
        }

        // --- ДЕЙСТВИЯ ---

        /**
         * Переходит на страницу входа.
         */
        public async Task NavigateAsync()
        {
            // Используем метод GoToUrlAsync из BasePage
            await GoToUrl(_path, "signin");
        }

        /**
         * Выполняет вход в систему, используя учетные данные.
         * Если логин/пароль не переданы, берутся из полей BasePage (Environment.GetEnvironmentVariable).
         */
        public async Task LoginAsync(string username = null, string password = null)
        {
            /*await Page.WaitForSelectorAsync(UsernameInput, new PageWaitForSelectorOptions
            {
                State = Microsoft.Playwright.WaitForSelectorState.Visible,
                Timeout = 15000 // Ждем до 15 секунд
            });*/

            // 1. Определяем учетные данные
            string login = username ?? Username;
            string pass = password ?? Password;

            // 2. Вводим логин. Используем FillFieldAsync из BasePage.
            await FillFieldAsync(UsernameInput, login, "Ввод логина");

            // 3. Вводим пароль.
            // Получаем ILocator для поля пароля.
            //var passwordLocator = Page.Locator(PasswordInput);
            await FillFieldAsync(PasswordInput, pass, "Ввод пароля");

            await ClickAsync(
            LoginButtonLocator,
            "Кнопка 'Войти' (Отправка формы)",
            expectedUrlPart: "dashboard" // <--- BasePage теперь ждет этот URL
             );


            /*// a) Вводим пароль
            await passwordLocator.FillAsync(pass);
            Console.WriteLine($"В поле пароля введено '{pass}'.");

            // b) Нажимаем Enter, чтобы отправить форму.
            // Playwright .NET использует PressAsync.
            await passwordLocator.PressAsync("Enter");
            Console.WriteLine("Нажат Enter для отправки формы.");*/

            // Делаем скриншот после отправки
            await TakeScreenshotAsync("Login_Form_Submitted");

            // Ждем успешного перехода (например, на дашборд)
            // Используем метод проверки URL из BasePage.
            await IsUrlContainsAsync("dashboard");
        }
    }
}