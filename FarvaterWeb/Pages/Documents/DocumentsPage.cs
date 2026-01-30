using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using FarvaterWeb.Tests.Counterparty;
using FarvaterWeb.Tests.Users;
using HarmonyLib;
using Microsoft.Playwright;
using Serilog;
using System.Drawing.Text;
using System.Web;

namespace FarvaterWeb.Pages.Documents
{
    public class DocumentsPage : BasePage
    {
        // Входящий документ
        private SmartLocator CreateDocumentButton => ButtonWithText("Создать документ");

        private SmartLocator DocumentTypeDropdown => Dropdown.WithLabel("Тип документа");

        private SmartLocator SummaryInputInIncomeDocument => Input.DescriptionField("Краткое описание *");

        private SmartLocator ProjectDropdown => Dropdown.WithLabel("Проект");

        private SmartLocator SenderDropdown => Dropdown.WithLabel("Отправитель");

        private SmartLocator SenderNumberInput => Input.WithLabel("Номер отправителя");

        private SmartLocator SenderSubscriberDropdown => Dropdown.WithLabel("От отправителя подписал");

        private SmartLocator PerformerDropdown => Dropdown.WithLabel("Исполнитель");

        private SmartLocator CancelButton => ButtonWithText("Отмена");

        private SmartLocator CreateButton => ButtonWithText("Создать");

        // Исходящий документ

        private SmartLocator AdresseesDropdown => Dropdown.WithLabel("Адресаты");

        private SmartLocator SignedByDropdown => Dropdown.WithLabel("Подписал");

        private SmartLocator SummaryInputInOutcomeDocument => Input.WithLabel("Краткое описание");

         

        //private SmartLocator CreatedDocumentInAList => Table.ClickActionInRow()

        private CalendarComponent PlanningResponseDate => new(Page, Log, _test, "Планируемая дата ответа", GetType().Name);

        //private SmartLocator FromDate =>
        public DocumentsPage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test)
        {
        }

        // Входящие документы

        public async Task ClickCreateDocumentButton()
        {
            await CreateDocumentButton.SafeClickAsync();
        }

        public async Task SelectDocumentType()
        {
            await DocumentTypeDropdown.SelectByIndexAndVerifyAsync(1);
        }

        public async Task FillIncomeDocumetDetails(IncomeDocumentDetails details)
        {
            await SummaryInputInIncomeDocument.ClearAndFillAsync(details.Summary);
            await SenderNumberInput.ClearAndFillAsync(details.SenderNumber);
        }

        public async Task FillSummaryInOutcomeDocument(OutcomeDocumentDetails details)
        {
            await SummaryInputInOutcomeDocument.ClearAndFillAsync(details.Summary);
        }

        public async Task SelectProject()
        {
            await ProjectDropdown.SelectByIndexAndVerifyAsync(0, isMultiSelect:true);
        }

        public async Task AppointPlanningResponseDate(DateTime date)
        {
            await PlanningResponseDate.SetDateAsync(date);
        }

        public async Task SelectSender()
        {
            await SenderDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task SelectAdressee()
        {
            await AdresseesDropdown.SelectByIndexAndVerifyAsync(0, customVerifyLocator: "td._table_cell_8wkbu_111");
        }

        public async Task SelectSenderSubscriber ()
        {
            await SenderSubscriberDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task SelectPerformer()
        {
            await PerformerDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task ClickCancelButton()
        {
            await CancelButton.SafeClickAsync();
        }

        public async Task ClickCreateButton()
        {
            await CreateButton.SafeClickAsync();
        }

        /*public async Task OpenCheckAndCloseCreatedDocument()
        {
            await 
        }*/



        

    }
}
