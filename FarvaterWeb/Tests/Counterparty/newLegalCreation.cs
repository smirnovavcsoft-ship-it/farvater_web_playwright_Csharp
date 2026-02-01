
using FarvaterWeb.Base;       
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

    [Collection("AllureCollection")]
    public class FarvaterLegalCreationTests : BaseTest
    {

        public FarvaterLegalCreationTests(ITestOutputHelper output) : base(output)
        {
        }
        [AllureOwner("AlexanderSmirnov")]
        [AllureSuite("Контрагенты")]       
        [AllureSubSuite("Юридические лица")]
        [Fact(DisplayName = "Проверка успешного создания нового юридического лица")]
        public async Task ShouldSuccessfullyCreateANewLegal()
        {
            var allureResultsPath = AllureLifecycle.Instance.ResultsDirectory;
            Log.Information($"Allure results path: {allureResultsPath}");

            try
            {
                Log.Information("--- Запуск сценария: Создание нового юр. лица ---");

                string login = Environment.GetEnvironmentVariable("LOGIN") ?? "SYSADMIN";
                string password = Environment.GetEnvironmentVariable("PASS") ?? "";

                var signInPage = new SignInPage(Page, Log, _test);
                var sideMenuPage = new SideMenuPage(Page, Log, _test);
                var counterpartyPage = new CounterpartyPage(Page, Log, _test);
                var newLegalPage = new NewLegalPage(Page, Log, _test);

                await signInPage.NavigateAsync();
                await signInPage.LoginAsync(login, password);


                await sideMenuPage.OpenSection("Контрагенты", "counterparty");

                await counterpartyPage.SelectPersonTypeAsync();

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

                await newLegalPage.FillForm(newLegalDetails);

                await newLegalPage.Create();

                await counterpartyPage.DeleteCounterparty(newLegalDetails.ShortName);

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