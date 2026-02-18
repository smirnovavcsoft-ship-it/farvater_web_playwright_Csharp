using FarvaterWeb.ApiServices;
using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using FarvaterWeb.TestData;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{
    [Collection("AllureCollection")]
    public class IncomeDocumentCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private DocumentsPage Documents => new DocumentsPage(Page, Log, _test);

        private CounterpartyApiService CounterpartyApi => new CounterpartyApiService(ApiRequest);
        public IncomeDocumentCreationTests(ITestOutputHelper output) : base(output)
        {

        }

        //[Fact(DisplayName ="Проверка успешного создания входящего документа")]
        [Theory(DisplayName = "Проверка успешного создания входящего документа")]
        [MemberData(nameof(IncomeDocumentTestData.GetUniversalContractCases), MemberType = typeof(IncomeDocumentTestData))]
        public async Task ShouldCreateIncomeDocument(UserModel actor, CounterpartyModel counterparty, IncomeDocumentDetails incomeDocument, string expectedResult)
        {
            // 1. Создаем через API

            string? counterpartyHandle = null;

            try
            {
                counterpartyHandle = await CounterpartyApi.PrepareCounterpartyAsync(counterparty.FullTitle, counterparty.ShortTitle, counterparty.Inn);

                Log.Information("--- Запуск сценария: Создание входящего документа---");
                await LoginAs(actor.Login!);
                await SideMenu.OpenSection("Входящие", "income");

                // Клик по кнопке "Создать документ"

                await Documents.ClickCreateDocumentButton();

                // Выбор типа документа



                // await Documents.SelectDocumentType();

                // Заполнение полей документа 

                // var incomeDocumentsDetails = new IncomeDocumentDetails(incomeDocument);

                await Documents.FillIncomeDocumetDetails(incomeDocument);

                // Нажатие кнопки "Отмена"

                await Documents.ClickCancelButton();

                // Клик по кнопке "Создать документ"

                await Documents.ClickCreateDocumentButton();

                // Выбор типа документа

                // await Documents.SelectDocumentType();

                // Выбор планируемой даты ответа

                DateTime date = new DateTime(2026, 02, 05);

                await Documents.AppointPlanningResponseDate(date);

                // Заполнение полей документа

                await Documents.FillIncomeDocumetDetails(incomeDocument);

                // Выбор проекта в выпадающем списке "Проект"

                await Documents.SelectProject();

                // Выбор отправителья в выпадающем списке "Отправитель*"

                //   await Documents.SelectSender();

                // Выбор даты "От"

                DateTime fromDate = new DateTime(2026, 01, 31);

                await Documents.AppointFromDate(fromDate);

                // Выбор "От отправителья подписал"



                // Выбор исполнителя



                // Нажатие кнопки "Создать"

                await Documents.ClickCreateButton();

                // Открытие и закрытие документа в списке (сделать, когда появится пропавший столбец "Описание" с кратким описанием внутри).

                //

                // Удаление документа (сделать, когда появится пропавший столбец "Описание" с кратким описанием внутри)
            }
            finally
            {
                // Если GUID был получен — удаляем
                if (!string.IsNullOrEmpty(counterpartyHandle))
                {
                    await CounterpartyApi.DeleteCounterpartyAsync(counterpartyHandle);
                }

            }

        }
    }
}
