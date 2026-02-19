using FarvaterWeb.ApiServices;
using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using FarvaterWeb.Generators;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using FarvaterWeb.TestData;
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
           // _counterparty = DataFactory.GenerateCounterparty();
           // _user = DataFactory.GenerateUser();
        }

        //[Fact(DisplayName = "Проверка успешного создания исходящего документа")]
        [Theory(DisplayName = "Проверка успешного создания исходящего документа")]
        [MemberData(nameof(OutcomeDocumentTestData.GetUniversalOutcomeDocumentCases), MemberType = typeof(OutcomeDocumentTestData))]

        public async Task ShouldCreateOutcomeDocment(UserModel actor, UserModel newUser, CounterpartyModel counterparty, OutcomeDocumentDetails outcomeDocument, string expectedResult)
        {
            /*var fullTitle = _counterparty.FullTitle;
            string shortTitle = _counterparty.ShortTitle;
            var inn = _counterparty.Inn;

            string postfix = DataPostfixExtensions.GetUniquePostfix();

            string lastName = _user.LastName!;
            string firstName = _user.FirstName!;
            string login = $"{lastName}{postfix}";*/

            string? userHandle = null;

            string? counterpartyHandle = null;

            // Создание контрагента и пользователя через API

            try
            {
                userHandle = await UserApi.PrepareUserAsync(newUser.LastName!, newUser.FirstName!, newUser.Login!);

                counterpartyHandle = await CounterpartyApi.PrepareCounterpartyAsync(counterparty.FullTitle, counterparty.ShortTitle, counterparty.Inn);

                Log.Information("--- Запуск сценария: Создание исходящего документа---");
                await LoginAs(actor.Login!);
                await SideMenu.OpenSection("Исходящие", "outcome");

                // Клик по кнопке "Создать документ"

                await Documents.ClickCreateDocumentButton();

                // Ввести текст в краткое описание

               // var outcomedocumentDetails = new OutcomeDocumentDetails("Кратакое описание краткое описание");

                await Documents.FillOutcomeDocumentDetails(outcomeDocument, newUser);

                // Выбор адресата

                //await Documents.SelectResipient(shortTitle);

                // Выбор исполнителя

               // await Documents.SelectPerformer(lastName, firstName);

                // Нажатие кнопки "Отмена"

                await Documents.ClickCancelButton();

                // Проверка несоздания документа (сейчас не сделать, потому что нет краткого описания документа в столбце описания, поэтому не к чему прицепиться)

                // Клик по кнопке "Создать документ"

                await Documents.ClickCreateDocumentButton();


                // Ввести текст в краткое описание

                await Documents.FillOutcomeDocumentDetails(outcomeDocument, newUser);

                // Выбор адресата

              //  await Documents.SelectResipient(shortTitle);

                // Выбор исполнителя

               // await Documents.SelectPerformer(lastName, firstName);

                // Заполнение полей адресата (пока не буду здесь заполнять, потому что какая-то таблица, которой возможно больше нигде не будет, если где-то встречу такую-же, то напишу)

                // Выбор проекта 

               // await Documents.SelectProject();

                // Выбор пользователя в поле "Подписал"

               // await Documents.SelectProject();

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
                    await UserApi.DismissUserAsync(userHandle);
                }
            }

        }

    }

}



        

