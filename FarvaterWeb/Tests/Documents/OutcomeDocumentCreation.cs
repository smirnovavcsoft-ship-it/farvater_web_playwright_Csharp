using FarvaterWeb.ApiServices;
using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using FarvaterWeb.Generators;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{
    public class OutcomeDocumentCreationTests : BaseTest
    {
        private CounterpartyApiService CounterpartyApi => new CounterpartyApiService(ApiRequest);
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private DocumentsPage Documents => new DocumentsPage(Page, Log, _test);

        private readonly CounterpartyModel _counterparty;

        private UserApiService UserApi => new UserApiService(ApiRequest);

        private readonly UserModel _user;
        public OutcomeDocumentCreationTests(ITestOutputHelper output) : base(output)
        {
            _counterparty = DataFactory.GenerateCounterparty();
            _user = DataFactory.GenerateUser();
        }

        [Fact(DisplayName = "Проверка успешного создания исходящего документа")]

        public async Task ShouldCreateOutcomeDocment()
        {
            var fullTitle = _counterparty.title;
            string shortTitle = _counterparty.shorttitle;
            var inn = _counterparty.inn;

            string postfix = DataPostfixExtensions.GetUniquePostfix();

            string lastName = _user.lastName!;
            string firstName = _user.firstName!;
            string login = $"{lastName}{postfix}";

            string? userHandle = null;

            string? counterpartyHandle = null;

            // Создание контрагента и пользователя через API

            try
            {
                userHandle = await UserApi.PrepareUserAsync(lastName, firstName, login);

                counterpartyHandle = await CounterpartyApi.PrepareCounterpartyAsync(fullTitle, shortTitle, inn);

                Log.Information("--- Запуск сценария: Создание исходящего документа---");
                await LoginAsAdmin();
                await SideMenu.OpenSection("Исходящие", "outcome");

                // Клик по кнопке "Создать документ"

                await Documents.ClickCreateDocumentButton();

                // Ввести текст в краткое описание

                var outcomedocumentDetails = new OutcomeDocumentDetails("Кратакое описание краткое описание");

                await Documents.FillSummaryInOutcomeDocument(outcomedocumentDetails);

                // Выбор адресата

                await Documents.SelectResipient(shortTitle);

                // Выбор исполнителя

                await Documents.SelectPerformer(lastName, firstName);

                // Нажатие кнопки "Отмена"

                await Documents.ClickCancelButton();

                // Проверка несоздания документа (сейчас не сделать, потому что нет краткого описания документа в столбце описания, поэтому не к чему прицепиться)

                // Клик по кнопке "Создать документ"

                await Documents.ClickCreateDocumentButton();


                // Ввести текст в краткое описание

                await Documents.FillSummaryInOutcomeDocument(outcomedocumentDetails);

                // Выбор адресата

                await Documents.SelectResipient(shortTitle);

                // Выбор исполнителя

                await Documents.SelectPerformer(lastName, firstName);

                // Заполнение полей адресата (пока не буду здесь заполнять, потому что какая-то таблица, которой возможно больше нигде не будет, если где-то встречу такую-же, то напишу)

                // Выбор проекта 

                await Documents.SelectProject();

                // Выбор пользователя в поле "Подписал"

                await Documents.SelectProject();

                // Нажатие кнопки "Создать"

                await Documents.ClickCreateButton();

                // Удаление документа (пока не сделать из-за отсутствия краткого описания в списке)



                // Обмновление страницы и проверка отсутствия документа (пока не сделать из-за отсутствия краткого описания в списке)


            }

            finally
            {
                // Если GUID был получен — удаляем
                if (!string.IsNullOrEmpty(counterpartyHandle))
                {
                    await CounterpartyApi.DeleteCounterpartyAsync(counterpartyHandle);
                }

                if (!string.IsNullOrEmpty(userHandle))
                {
                    await CounterpartyApi.DeleteCounterpartyAsync(userHandle);
                }
            }

        }

    }

}



        

