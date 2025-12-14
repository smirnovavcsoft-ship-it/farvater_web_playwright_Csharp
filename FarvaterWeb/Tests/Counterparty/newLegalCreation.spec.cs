
// Предполагаемые namespace для Page Objects
using FarvaterWeb.Pages;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Setup;
using Microsoft.Playwright;
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
    [Collection("Playwright Test Collection")]
    public class FarvaterLegalCreationTests
    {
        // IPage и ITestOutputHelper внедряются через конструктор XUnit.
        private readonly IPage _page;
        private readonly string _baseUrl /*= "https://farvater.mcad.dev/farvater/"*/;
        private readonly ITestOutputHelper _output;

        private readonly PlaywrightFixture _fixture;

        public FarvaterLegalCreationTests(PlaywrightFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _page = fixture.Page;
            _baseUrl = fixture.BaseUrl;
            _output = output;
        }

        [Fact(DisplayName = "Проверка успешного создания нового юридического лица")]
        public async Task ShouldSuccessfullyCreateANewLegal()
        {
            _output.WriteLine("---Начало тестового сценария: Создание нового юр. лица---");

            string login = _fixture.GlobalUsername;
            string password = _fixture.GlobalPassword;

            //--- Инициализация Page Objects ----
            var signInPage = new SignInPage(_page, _baseUrl, login, password);
            var dashboardPage = new DashboardPage(_page, _baseUrl, login, password);
            var counterpartyPage = new CounterpartyPage(_page, _baseUrl, login, password);
            var newLegalPage = new NewLegalPage(_page, _baseUrl, login, password);

            string createdLegalId = null;

            // 1. Вход в систему и проверка URL
            _output.WriteLine("1. Вход в систему и проверка URL");
            await signInPage.NavigateAsync();
            await signInPage.LoginAsync();
            Assert.Contains("dashboard", _page.Url);

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