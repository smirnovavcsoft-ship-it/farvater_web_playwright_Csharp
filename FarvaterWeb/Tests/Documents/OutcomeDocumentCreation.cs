using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{
    public class OutcomeDocumentCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private DocumentsPage Documents => new DocumentsPage(Page, Log, _test); 
        public OutcomeDocumentCreationTests(ITestOutputHelper output) : base(output) { }
        
        [Fact(DisplayName ="Проверка успешного создания исходящего документа")]

        public async Task ShouldCreateOutcomeDocment ()
        {
            Log.Information("--- Запуск сценария: Создание исходящего документа---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Исходящие", "outcome");

            await Documents.ClickCreateDocumentButton();

            var outcomedocumentDetails = new OutcomeDocumentDetails("Кратакое описание краткое описание");

            await Documents.FillSummaryInOutcomeDocument(outcomedocumentDetails);

            await Documents.SelectAdressee();

            await Documents.SelectPerformer();

            await Documents.ClickCancelButton();

            await Documents.ClickCreateDocumentButton();


            await Documents.FillSummaryInOutcomeDocument(outcomedocumentDetails);

            await Documents.SelectAdressee();

            await Documents.SelectPerformer();

            await Documents.SelectProject();

            await Documents.SelectProject();

            await Documents.ClickCreateButton();


        }



    }
}
