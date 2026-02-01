using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{
    [Collection("AllureCollection")]
    public class IncomeDocumentCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private DocumentsPage Documents => new DocumentsPage(Page, Log, _test);
        public IncomeDocumentCreationTests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact(DisplayName ="Проверка успешного создания входящего документа")]
        public async Task ShouldCreateIncomeDocument ()
        {
            Log.Information("--- Запуск сценария: Создание входящего документа---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Входящие", "income");

            await Documents.ClickCreateDocumentButton();

            

            await Documents.SelectDocumentType();

            var incomeDocumentsDetails = new IncomeDocumentDetails("Краткое содержание, краткое содержание", "14645");

            await Documents.FillIncomeDocumetDetails(incomeDocumentsDetails);

            await Documents.ClickCancelButton();

            await Documents.ClickCreateDocumentButton();

            await Documents.SelectDocumentType();

            DateTime date = new DateTime(2026, 02, 05);

            await Documents.AppointPlanningResponseDate(date);

            await Documents.FillIncomeDocumetDetails(incomeDocumentsDetails);

            await Documents.SelectProject();

            await Documents.SelectSender();

            DateTime fromDate = new DateTime(2026, 01, 31);

            await Documents.AppointFromDate(fromDate);



            

            await Documents.ClickCreateButton();

        }

    }
}
