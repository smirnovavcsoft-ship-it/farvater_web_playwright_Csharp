
using FarvaterWeb.Base; // Путь к вашему BaseTest и моделям
using FarvaterWeb.Pages;
using FarvaterWeb.Pages.Common;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using Allure.Xunit;
using Allure.Xunit.Attributes;
using Allure.Net.Commons;



namespace FarvaterWeb.Tests.Counterparty
{
    // Модель данных (record) остается, это отличная практика
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

    // Мы наследуем BaseTest, который берет на себя:
    // 1. Создание Browser, Context, Page
    // 2. Инициализацию Serilog
    // 3. Запись Видео и Скриншотов при падении
    
    //[AllureXunit]
    public class FarvaterLegalCreationTests : BaseTest
    {

        // Конструктор ОБЯЗАТЕЛЬНО должен принимать output и отдавать его в base
        public FarvaterLegalCreationTests(ITestOutputHelper output) : base(output)
        {
        }
        //[Allure.Xunit.Attributes.AllureXunit]
        [AllureOwner("AlexanderSmirnov")]
        [AllureSuite("Контрагенты")] // Это будет главная папка в отчете
        //[AllureXunit(DisplayName = "Проверка успешного создания нового юридического лица")]
        //[AllureXunit]
        //[AllureXunit]
        //[AllureDisplayContext("Создание нового юр. лица")]

        [Fact(DisplayName = "Проверка успешного создания нового юридического лица")]
        public async Task ShouldSuccessfullyCreateANewLegal()
        {
            var allureResultsPath = AllureLifecycle.Instance.ResultsDirectory;
            Log.Information($"Allure results path: {allureResultsPath}");

            try
            {
                Log.Information("--- Запуск сценария: Создание нового юр. лица ---");

                // Данные берем из переменных окружения или конфига (через BaseTest если нужно)
                string login = Environment.GetEnvironmentVariable("LOGIN") ?? "SYSADMIN";
                string password = Environment.GetEnvironmentVariable("PASS") ?? "";

                // 1. Инициализация Page Objects
                // Передаем Page и Log, которые достались нам от BaseTest
                var signInPage = new SignInPage(Page, Log, _test);
                var sideMenuPage = new SideMenuPage(Page, Log, _test);
                var counterpartyPage = new CounterpartyPage(Page, Log, _test);
                var newLegalPage = new NewLegalPage(Page, Log, _test);

                // 2. Шаги теста
                await signInPage.NavigateAsync();
                await signInPage.LoginAsync(login, password);


                await sideMenuPage.OpenSection("Контрагенты", "counterparty");

                // Клик по "Добавить" -> "Юр. лицо"
                await counterpartyPage.SelectPersonTypeAsync();

                //await Assertions.Expect(Page.GetByText("Этого текста нет на странице")).ToBeVisibleAsync();

                // 3. Подготовка данных
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

                // 4. Заполнение и проверка
                await newLegalPage.FillForm(newLegalDetails);

                /*// Используем встроенные ассерты Playwright (они умеют ждать и делают скриншоты при ошибке)
                await Assertions.Expect(Page.Locator("input[name='inn']")).ToHaveValueAsync(newLegalDetails.Inn);
                await Assertions.Expect(Page.Locator("input[name='shorttitle']")).ToHaveValueAsync(newLegalDetails.ShortName);
                await Assertions.Expect(Page.Locator("input[name='title']")).ToHaveValueAsync(newLegalDetails.FullName);
                await Assertions.Expect(Page.Locator("input[name='address']")).ToHaveValueAsync(newLegalDetails.Address);
                await Assertions.Expect(Page.Locator("input[name='ogrn']")).ToHaveValueAsync(newLegalDetails.Ogrn);
                await Assertions.Expect(Page.Locator("input[name='kpp']")).ToHaveValueAsync(newLegalDetails.Kpp);
                await Assertions.Expect(Page.Locator("input[name='phone']")).ToHaveValueAsync(newLegalDetails.Phone);
                await Assertions.Expect(Page.Locator("input[name='email']")).ToHaveValueAsync(newLegalDetails.Email);*/

                //5. Нажатие кнопки "Создать"

                await newLegalPage.Create();

                // Нет создания контактов и дополнительных контактов. Потом допишу. Наверно.

                await counterpartyPage.DeleteCounterparty(newLegalDetails.ShortName);

                Log.Information("Тест успешно завершен.");

                // Если мы дошли до сюда без исключений, помечаем для BaseTest, что видео можно удалить (опционально)
                MarkTestAsPassed();
            }
            catch (Exception ex)
            {
                // Передаем текст ошибки прямо в отчет перед тем, как «уронить» тест для xUnit
                _test.Fail($"<b>Критическая ошибка:</b> {ex.Message}<br>StackTrace: {ex.StackTrace}");
                throw; // Пробрасываем ошибку дальше, чтобы xUnit пометил тест красным
            }

            /*// Это принудительно создаст запись в Allure, даже если атрибут не сработал
            AllureLifecycle.Instance.WrapInStep(() => {
                Log.Information("Проверка связи с Allure");
            }, "Степ-проверка");*/

        }
    }

   
}