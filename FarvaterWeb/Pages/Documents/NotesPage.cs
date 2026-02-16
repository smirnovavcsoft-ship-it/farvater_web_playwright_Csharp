using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.Pages.Documents
{
    public class NotesPage : BasePage
    {
        private TableComponent Table => new TableComponent(Page, Log, _test);
        private SmartLocator CreateDocumentButton => ButtonWithText("Создать документ");
        private SmartLocator DocumentTypeDropdown => Dropdown.WithLabel("Тип документа");

        private SmartLocator TopicInput => Input.WithLabel("Тема");

        private SmartLocator ContentInput => Input.DescriptionField("Содержание *");

        private SmartLocator AdresseesDropdown => Dropdown.WithInputAndLabel("Адресаты");

        private SmartLocator CancelButton => ButtonWithText("Отмена");

        private SmartLocator CreateButton => ButtonWithText("Создать");

        public NotesPage(IPage page, ILogger logger, ExtentTest _test) : base(page, logger, _test) { }

        public async Task ClickCreateDocumentButton ()
        {
            await CreateDocumentButton.ClickCreateButtonAndWait();
        }

        public async Task FillNoteDetails (NoteDetails details)
        {
            string text = "";
            await DocumentTypeDropdown.SelectByTextAndVerifyAsync(details.DocumentType);
            await TopicInput.ClearAndFillAsync(details.Topic);
            await ContentInput.ClearAndFillAsync(details.Content);
            await AdresseesDropdown.SelectUserAndVerifyAsync( details.LastName, details.FirstName);
        }

        public async Task CancelAndVerify(string topic)
        {
            await CancelAction.CancelAndVerify(topic);
        }

        public async Task ClickCreateButton()
        {
            await CreateButton.ClickCreateButtonAndWait();
        }

        public async Task DeleteCreatedNote(string topic)
        {
            string buttonText = "Удалить";
            await Table.DeleteRow(topic, buttonText);
        }

        
    }
}
