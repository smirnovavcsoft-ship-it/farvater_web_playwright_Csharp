
// Предполагаемые namespace для Page Objects
using FarvaterWeb.Pages.Common;
using Microsoft.Playwright;
using FarvaterWeb.Pages;
// Используем TestOutputHelper для вывода в консоль XUnit
using Xunit.Abstractions;


namespace FarvaterWeb.Tests.Counterparty
{
    // --- Вспомогательный класс для данных ---
    public record LegalDetails(
        string Inn,
        string ShortName,
        string FullName,
        string Address,
        string Ogrn,
        string Kpp,
        string Phone,
        string Email
    );

    // --- Класс теста ---
    // Наследуемся от PageTest (если используете фреймворк Playwright.XUnit)
    // или используем IPage в конструкторе, как показано ниже.
    public class FarvaterLegalCreationTests
    {
        // IPage и ITestOutputHelper внедряются через конструктор XUnit.
        private readonly IPage _page;
        private readonly string _baseUrl /*= "https://farvater.mcad.dev/farvater/"*/;
        private readonly ITestOutputHelper _output;

        public FarvaterLegalCreationTests(IPage page, string baseUrl, ITestOutputHelper output)
        {
            _page = page;
            _baseUrl = baseUrl;
            _output = output;
        }

        [Fact(DisplayName = "Проверка успешного создания нового юридического лица")]
        public async Task ShouldSuccessfullyCreateANewLegal()
        {
            _output.WriteLine("---Начало тестового сценария: Создание нового юр. лица---");

            //--- Инициализация Page Objects ----
            var signInPage = new SignInPage(_page, _baseUrl);
            var dashboardPage = new DashboardPage(_page, _baseUrl);
            var counterpartyPage = new CounterpartyPage(_page, _baseUrl);
            var newLegalPage = new NewLegalPage(_page, _baseUrl);

            string createdLegalId = null;

            // 1. Вход в систему и проверка URL
            _output.WriteLine("1. Вход в систему и проверка URL");
            await signInPage.NavigateAsync();

            // Проверка URL в XUnit с использованием Asser.Contains
            Assert.Contains("signin", _page.Url);

            // 2. Авторизация и проверка перехода на Главную
            _output.WriteLine("2. Авторизация и проверка перехода на Главную");
            await signInPage.LoginAsync();
            Assert.Contains("dashboard", _page.Url);

            // 3. Переход на вкладку "Контрагенты"
            _output.WriteLine("3. Переход на вкладку 'Контрагенты' и проверка URL");
            await dashboardPage.NavigateToCounterpartyAsync();
            Assert.Contains("counterparty", _page.Url);

            // 4. Открытие выпадающего списка выбора типа контрагента
            _output.WriteLine("4. Открытие выпадающего списка выбора типа контрагента");
            await counterpartyPage.SelectPersonTypeAsync();
            _output.WriteLine("Тип контрагента успешно выбран.");
            Assert.Contains("newlegal", _page.Url);
            await newLegalPage.WaitForFormToLoadAsync();

            // 5. Заполнение полей нового контрагента
            _output.WriteLine("5. Заполнение полей нового контрагента");

            var newLegalDetails = new LegalDetails(
                Inn: "7703010336",
                ShortName: "ООО «Вектор»",
                FullName: "Общество с ограниченной ответственностью «Вектор»",
                Address: "125009, г. Москва, ул. Тверская, д. 9, стр. 7",
                Ogrn: "1027739194247",
                Kpp: "770301001",
                Phone: "+7 (495) 123-45-67",
                Email: "info@vektor.ru"
            );

            await newLegalPage.FillNewLegalDetailsAsync(newLegalDetails);
            var filledDetails = await newLegalPage.GetNewLegalDetailsAsync();

            // Проверка полей
            Assert.Equal(newLegalDetails.Inn, filledDetails.Inn);
            Assert.Equal(newLegalDetails.ShortName, filledDetails.ShortName);
            // ... добавьте Assert.Equal для всех остальных полей

            _output.WriteLine("Проверка заполненных данных завершена.");
        }
    }
}