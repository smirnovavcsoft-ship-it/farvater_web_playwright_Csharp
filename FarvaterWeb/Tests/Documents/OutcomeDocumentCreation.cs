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

            // Клик по кнопке "Создать документ"

            await Documents.ClickCreateDocumentButton();

            // Ввести текст в краткое описание

            var outcomedocumentDetails = new OutcomeDocumentDetails("Кратакое описание краткое описание");

            await Documents.FillSummaryInOutcomeDocument(outcomedocumentDetails);

            // Выбор адресата

            await Documents.SelectAdressee();

            // Выбор исполнителя

            await Documents.SelectPerformer();

            // Нажатие кнопки "Отмена"

            await Documents.ClickCancelButton();

            // Проверка несоздания документа (сейчас не сделать, потому что нет краткого описания документа в столбце описания, поэтому не к чему прицепиться)

            // Клик по кнопке "Создать документ"

            await Documents.ClickCreateDocumentButton();


            // Ввести текст в краткое описание

            await Documents.FillSummaryInOutcomeDocument(outcomedocumentDetails);

            // Выбор адресата

            await Documents.SelectAdressee();

            // Выбор исполнителя

            await Documents.SelectPerformer();

            // Заполнение полей адресата (пока не буду здесь заполнять, потому что какая-то таблица, которой возможно больше нигде не будет, если где-то встречу такую-же, то напишу)

            // Выбор проекта 

            await Documents.SelectProject();

            // Выбор пользователя в поле "Подписал"

            await Documents.SelectProject();

            // Нажатие кнопки "Создать"

            await Documents.ClickCreateButton();

            // Обновление страницы и проверка создания документа в списке (пока не сделать из-за отсутствия краткого описания в списке)

            // Удаление документа (пока не сделать из-за отсутствия краткого описания в списке)

            // Обмновление страницы и проверка отсутствия документа (пока не сделать из-за отсутствия краткого описания в списке)


        }



    }
}
