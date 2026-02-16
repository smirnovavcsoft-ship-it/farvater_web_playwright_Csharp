using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using Serilog;

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

        private SmartLocator ResipientsDropdown => Dropdown.WithLabel("Адресаты");

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

        public CancelComponent CancelAction => new CancelComponent(Page, Log, _test);

        //private SmartLocator CreatedDocumentInAList => Table.ClickActionInRow()

        private DateComponent Date(string label) => new(Page, Log, _test, label, GetType().Name);

        private RangeComponent Range(string label) => new(Page, Log, _test, label, GetType().Name);

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
            await ProjectDropdown.SelectByIndexAndVerifyAsync(0, isMultiSelect: true);
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

        public async Task SelectResipient(string shortTitle)
        {
            // await ResipientsDropdown.SelectByIndexAndVerifyAsync(0, customVerifyLocator: "td._table_cell_8wkbu_111");
            await ResipientsDropdown.SelectByTextAndVerifyAsync(shortTitle, customVerifyLocator: "td._table_cell_8wkbu_111");
        }

        public async Task SelectSenderSubscriber()
        {
            await SenderSubscriberDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task SelectPerformer(string lastName, string firstName)
        {
            await PerformerDropdown.SelectUserAndVerifyAsync(lastName, firstName);
        }

        public async Task ClickCancelButton()
        {
            await CancelButton.SafeClickAsync();
        }

        public async Task ClickCreateButton()
        {
            var creationUrl = Page.Url;
            await CreateButton.SafeClickAsync();         
                        
            // Ждем, пока URL перестанет быть равен creationUrl
            await Page.WaitForFunctionAsync($"() => window.location.href !== '{creationUrl}'");

            // Теперь мы точно знаем, что редирект случился. 
            // Можем даже вывести новый URL в лог для интереса:
            Log.Information($"Система перенаправила нас на: {Page.Url}");
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

        /*public async Task SelectParty1(string shortTitle)
        {
            await Party1Dropdown.SelectByTextAndVerifyAsync(shortTitle);
        }*/

        public async Task SelectParty2(string shortTitle)
        {
            await Party2Dropdown.SelectByTextAndVerifyAsync(shortTitle);
        }

        public async Task AppointContractTerm(DateTime startDate, DateTime endDate)
        {
            await Range("Сроки по договору").SetStartDateAsync(startDate);
            await Range("Сроки по договору").SetEndDateAsync(endDate);
        }

        public async Task SelectParty1(string shortTitle)
        {
            string stepName = $"[DocumentsPage] Выбор Стороны 1: '{shortTitle}'";

            await Do(stepName, async () =>
            {
                // 1. Открываем список
                await Party1Dropdown.Locator.ClickAsync(new() { Force = true });

                // 2. Находим контейнер с опциями (только видимый)
                var optionsContainer = Page
                    .Locator("[data-testid='dropdown_list-options']:visible")
                    .Last;

                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                // 3. Выбираем нужный пункт по точному тексту
                // Здесь оставляем Exact = true, чтобы кликнуть именно туда, куда нужно
                var targetOption = optionsContainer
                    .Locator("[data-signature='dropdown_list-item']")
                    .GetByText(shortTitle, new() { Exact = true })
                    .Last;

                await targetOption.ClickAsync();

                // 4. СПЕЦПРОВЕРКА:
                // Мы берем текст из всего контейнера, но проверяем только наличие искомой строки.
                // Это игнорирует "Сторона 1 *" и не ломается из-за Regex или Exact Match.
                await Assertions.Expect(Party1Dropdown.Locator).ToContainTextAsync(shortTitle);

                // 5. Ждем закрытия списка для стабильности
                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
            });
        }

        public async Task ClickCancelButtonAndVarify(string shortTitle)
        {
            await CancelAction.CancelAndVerify(shortTitle);
        }

        public async Task DeleteCreatedContract(string shortTitle)
        {            
            string buttonText = "Удалить";
            await Table.DeleteRow(shortTitle, buttonText);

        }

        /*public async Task DeleteCreatedOutcomeDocument(string shortTitle)
        {
            string buttonText = "Удалить";
            await Table.DeleteRow(shortTitle, buttonText);
        }*/

        
    }
}