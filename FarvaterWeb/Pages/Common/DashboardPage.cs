using Microsoft.Playwright;
using FarvaterWeb.Pages.Common; // Предполагаем, что BasePage находится здесь
using System.Threading.Tasks;
using System.IO;
using Microsoft.Playwright.Core; // Может потребоваться для некоторых расширений
using System; // Для исключений


namespace FarvaterWeb.Pages.Common
{
    // Класс страницы Дашборда
    public class DashboardPage : BasePage
    {
        // Относительный путь к странице
        private readonly string _path = "/dashboard";

        // --- Локаторы Playwright (string) ---
        private class DashboardLocators
        {
            public const string Dashboard = "//a[contains(@href, \"dashboard\")]";
            public const string ProjectsButton = "//a[contains(@href, \"projects\")]";
            public const string CounterpartyButton = "//a[contains(@href, \"counterparty\")]";
            public const string AssignmentsButton = "//a[contains(@href, \"assignments\")]";
            public const string AssignmentButton = "//button[contains(., \"Поручение\")]";
            public const string UsersButton = "//a[contains(@href, \"users\")]";
            public const string UserDropdown = "//div[starts-with(@class, '_user_name_')]";
            public const string LogoutItem = "//div[@data-signature='dropdown-menu-item' and .//div[text()='Выйти']]";
            public const string AssignmentsAccordion = "//div[@data-signature='accordion-wrapper']//span[text()='Поручения']";
        }

        public DashboardPage(IPage page, string baseUrl) : base(page, baseUrl)
        {
            // Конструктор инициализирует родительский класс BasePage.
        }

        // --- ДЕЙСТВИЯ ---

        /*public async Task NavigateToProjectsAsync()
        {
            await ClickAsync(DashboardLocators.ProjectsButton, "Вкладка 'Проекты'");
            await IsUrlContainsAsync("projects");
        }*/

        public async Task NavigateToProjectsAsync()
        {
            // C# автоматически вызывает ClickAsync(ILocator locatorObject, string actionDescription)
            await ClickAsync(DashboardLocators.ProjectsButton, "Вкладка 'Проекты'");
            await IsUrlContainsAsync("projects");
        }

        /*public async Task NavigateToCounterpartyAsync()
        {
            await ClickAsync(DashboardLocators.CounterpartyButton, "Вкладка 'Контрагенты'");
            await IsUrlContainsAsync("counterparty");
        }*/

        public async Task NavigateToCounterpartyAsync()
        {
            // Используем ClickAsync с третьим параметром, 
            // который указывает, что после клика мы ожидаем переход на URL,
            // содержащий "counterparty".
            // Вся логика ожидания, логирования и скриншота теперь инкапсулирована в BasePage.

            await ClickAsync(
                DashboardLocators.CounterpartyButton,
                "Вкладка 'Контрагенты' и переход на страницу", // Улучшенное описание
                expectedUrlPart: "counterparty" // <--- НОВЫЙ ПАРАМЕТР
            );

            // Метод IsUrlContainsAsync больше не нужен, так как проверка уже выполнена внутри ClickAsync.
        }

        public async Task NavigateToAssignmentsAsync()
        {
            await ClickAsync(DashboardLocators.AssignmentsButton, "Вкладка \"Поручения\"");
            await IsUrlContainsAsync("assignments");
        }

        public async Task ClickAssignmentButtonAsync()
        {
            await ClickAsync(DashboardLocators.AssignmentButton, "Кнопка \"Поручение\"");
        }

        public async Task NavigateToUsersAsync()
        {
            await ClickAsync(DashboardLocators.UsersButton, "Вкладка \"Пользователи\"");
            await IsUrlContainsAsync("users");
        }

        /**
         * Выполняет выход из системы, включая ожидание появления элемента меню.
         */
        public async Task LogoutAsync()
        {
            // 1. Клик, чтобы открыть меню
            await Page.Locator(DashboardLocators.UserDropdown).ClickAsync();
            Console.WriteLine("Клик по элементу: Раскрывающееся меню пользователя");

            // 2. *** ДОБАВЛЯЕМ ЯВНОЕ ОЖИДАНИЕ ВИДИМОСТИ ***
            // Playwright for .NET использует WaitForStateAsync.
            await Page.WaitForSelectorAsync(DashboardLocators.LogoutItem, new PageWaitForSelectorOptions
            {
                // Ожидаем видимости
                State = Microsoft.Playwright.WaitForSelectorState.Visible,
                Timeout = 10000
            });


            // 3. Клик по пункту "Выйти"
            await Page.Locator(DashboardLocators.LogoutItem).ClickAsync();
            Console.WriteLine("Клик по элементу: Пункт \"Выйти\"");

            // Если нужно, добавьте проверку URL:
            // await IsUrlContainsAsync("signin"); 
        }
    }
}