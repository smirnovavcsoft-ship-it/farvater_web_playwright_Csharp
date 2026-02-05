using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using FarvaterWeb.Services;
using FarvaterWeb.Tests.Counterparty;
using FarvaterWeb.Tests.Users;
using HarmonyLib;
using Microsoft.Playwright;
using Serilog;
using Serilog.Core;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
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

        // Договоры

        private SmartLocator CreateContractButton => ButtonWithText("Создать договор");

        private SmartLocator ContractSubjectInput => Input.WithLabel("Предмет договора");

        private SmartLocator ContractTypeDropdown => Dropdown.WithLabel("Тип договора");

        private SmartLocator Party1Dropdown => Dropdown.WithLabel("Сторона 1");

        private SmartLocator Party2Dropdown => Dropdown.WithLabel("Сторона 2");

        private SmartLocator Party1NameInput => Input.WithLocator(Page.Locator("input[name='party1Name']"), "Именуемый в дальнейшем");

        private SmartLocator Party2NameInput => Input.WithLocator(Page.Locator("input[name='party2Name']"), "Именуемый в дальнейшем");

        private SmartLocator CostInput => Input.WithLabel("Стоимость, ₽");

        private SmartLocator WithNDSInput => Input.WithLabel("В том числе НДС, ₽");

        private SmartLocator TotalCostInput => Input.WithLabel("Полная стоимость, ₽");







        //private SmartLocator CreatedDocumentInAList => Table.ClickActionInRow()

        private DateComponent Date (string label) => new(Page, Log, _test, label , GetType().Name);

        private RangeComponent Range (string label) => new(Page, Log, _test, label, GetType().Name);

        


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
            await Date("Планируемая дата ответа").SetDateAsync(date);
        }

        public async Task AppointFromDate(DateTime date)
        {
            await Date("От").SetDateAsync(date);
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

        // Договоры

        public async Task ClickCreateContractButton()
        {
            await CreateContractButton.SafeClickAsync();
        }

        public async Task SelectContractType()
        {
            await ContractTypeDropdown.SelectByIndexAndVerifyAsync(0);
        }

        

        public async Task FillContractDetails(ContractDetails details)
        {
            await ContractSubjectInput.ClearAndFillAsync(details.ContractSubject);
            await Party1NameInput.ClearAndFillAsync(details.Party1Name);
            await Party2NameInput.ClearAndFillAsync(details.Party2Name);
            await CostInput.ClearAndFillAsync(details.Cost);
            await WithNDSInput.ClearAndFillAsync(details.WithNDS);
            await TotalCostInput.ClearAndFillAsync(details.TotalCost);
        }

        public async Task SelectParty1(string shortTitle)
        {
            await Party1Dropdown.SelectByTextAndVerifyAsync(shortTitle);
        }

        public async Task SelectParty2()
        {
            await Party2Dropdown.SelectByIndexAndVerifyAsync(1);
        }

        public async Task AppointContractTerm(DateTime startDate, DateTime endDate)
        {
            await Range("Сроки по договору").SetStartDateAsync(startDate);
            await Range("Сроки по договору").SetEndDateAsync(endDate);
        }

        /*public async Task PrepareCounterpartyAsync(string title, string shortTitle, string inn)
        {
            await Do($"[API] Создание контрагента: {shortTitle} (ИНН: {inn})", async () =>
            {
                // Создаем модель. Остальные поля (адрес, телефон и т.д.) 
                // подтянутся как пустые строки из определений в классе CounterpartyModel.
                var counterparty = new CounterpartyModel
                {
                    inn = inn,
                    title = title,           // Полное наименование (например, Общество с ограниченной...)
                    shorttitle = shortTitle, // Краткое наименование (например, ООО "Ромашка")
                    type = "LEGALENTITY_DEF"
                };

                var response = await Api.CreateCounterpartyAsync(counterparty);

                if (!response.Ok)
                {
                    var errorBody = await response.TextAsync();

                    // Логируем, если контрагент уже есть — для теста это не проблема
                    if (response.Status == 400 || response.Status == 409)
                    {
                        //Logger.Info($"Контрагент с ИНН {inn} уже существует в системе.");
                        return;
                    }

                    throw new Exception($"Не удалось создать контрагента через API. Status: {response.Status}, Body: {errorBody}");
                }
            });
        }*/

       







    }
}
