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
        private SmartLocator CreateDocumentButton => ButtonWithText("Создать документ");
        private SmartLocator DocumentTypeDropdown => Dropdown.WithLabel("Тип документа");

        private SmartLocator TopicInput => Input.WithLabel("Тема");

        private SmartLocator ContentInput => Input.WithLabel("Содержание");

        private SmartLocator AdresseesDropdown => Dropdown.WithLabel("Адресаты");

        private SmartLocator CancelButton => ButtonWithText("Отмена");

        private SmartLocator CreateButton => ButtonWithText("Создать");

        public NotesPage(IPage page, ILogger logger, ExtentTest _test) : base(page, logger, _test) { }

        public async Task ClickCreateDocumentButton ()
        {
            await CreateDocumentButton.SafeClickAsync();
        }

        public async Task FillNoteDetails (NoteDetails details)
        {
            //await DocumentTypeDropdown.
        }
    }
}
