using FarvaterWeb.ApiServices;
using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using FarvaterWeb.Generators;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using FarvaterWeb.TestData;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;


namespace FarvaterWeb.Tests.Documents
{


    public class NoteCreationTests : BaseTest
    {
        private LoginPage LoginForm => new LoginPage(Page, Log, _test);
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private DocumentsPage Documents => new DocumentsPage(Page, Log, _test);

        private NotesPage Notes => new NotesPage(Page, Log, _test);

        private UserApiService UserApi => new UserApiService(ApiRequest);

        //private readonly UserModel _user;

        public NoteCreationTests(ITestOutputHelper output) : base(output)
        {
            //_user = DataFactory.GenerateUser();
        }

        //[Fact(DisplayName = "Проверка успешного создания записки")]
        [Theory(DisplayName = "Проверка успешного создания записки")]
        [MemberData(nameof(NoteTestData.GetUniversalNoteCases), MemberType = typeof(NoteTestData))]
        public async Task ShouldCreateNote(UserModel actor, UserModel newUser, NoteDetails note, string expectedResult)
        {
            // Создание пользователя через API

            //string postfix = DataPostfixExtensions.GetUniquePostfix();

            //string lastName = _user.LastName!;
            //string firstName = _user.FirstName!;
            //string login = $"{lastName}{postfix}";

            string? userHandle = null;

            try

            {
                userHandle = await UserApi.PrepareUserAsync(newUser.LastName!, newUser.FirstName!, newUser.Login!);


                Log.Information("--- Запуск сценария: Создание записки---");

                try
                {
                  await LoginAs(actor.Login!);
                }
                catch
                {
                  await LoginAsAdmin();
                }
                await SideMenu.OpenSection("Записки", "notes");

                // Нажатие кнопки "Создание документа"

                await Notes.ClickCreateDocumentButton();


                // Ввод и выбор данных (Тип документа, Тема, Содержание, Адресаты, ). Проект пока выбирать не буду. Потом добавлю, когда разберусь с API

                /*var noteDetails = new NoteDetails
                    (
                    note.DocumentType,
                    note.Topic,
                    note.Content,
                    user.LastName,
                    user.FirstName
                     );*/

                await Notes.FillNoteDetails(note);

                // Нажатие кнопки "Отмена"

                await Notes.CancelAndVerify(note.Topic);


                // Нажатие кнопки "Создание документа"

                await Notes.ClickCreateDocumentButton();


                // Ввод и выбор данных (Тип документа, Тема, Содержание, Адресаты, ). Проект пока выбирать не буду. Потом добавлю, когда разберусь с API

                await Notes.FillNoteDetails(note);

                // Нажатие кнопки "Создать"

                await Notes.ClickCreateButton();

                // Удаление записки

                await SideMenu.OpenSection("Записки", "notes");
                await Notes.DeleteCreatedNote(note.Topic);
            }

            // Удалить адресата из базы
            finally
            {
                // Если GUID был получен — удаляем
                if (!string.IsNullOrEmpty(userHandle))
                {
                    await UserApi.DismissUserAsync(userHandle);
                }

            }



            // Удалить проект из базы ( пока проект в записку не добавляю, нужно разбираться с созданием проекта через api


        }
    }
}
