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
        private SmartLocator CreateDocumentButton => ButtonWithText("Создать документ");

        private SmartLocator DocumentTypeDropdown => Dropdown.WithLabel("Тип документа");

        private SmartLocator SummaryInput => Input.DescriptionField("Краткое описание *");

        private SmartLocator ProjectDropdown => Dropdown.WithLabel("Проект");

        private SmartLocator SenderDropdown => Dropdown.WithLabel("Отправитель");

        private SmartLocator SenderNumberInput => Input.WithLabel("Номер отправителя");

        private SmartLocator SenderSubscriberDropdown => Dropdown.WithLabel("От отправителя подписал");

        private SmartLocator PerformerDropdown => Dropdown.WithLabel("Исполнитель");

        private SmartLocator CancelButton => ButtonWithText("Отмена");

        private SmartLocator CreateButton => ButtonWithText("Создать");

        //private SmartLocator CreatedDocumentInAList => Table.ClickActionInRow()

        //private SmartLocator PlanningResponseDate =>

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
            await SummaryInput.ClearAndFillAsync(details.Summary);
            await SenderNumberInput.ClearAndFillAsync(details.SenderNumber);
        }

        public async Task SelectProject()
        {
            await ProjectDropdown.SelectByIndexAndVerifyAsync(0, isMultiSelect:true);
        }

        public async Task SelectSender()
        {
            await SenderDropdown.SelectByIndexAndVerifyAsync(0);
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
