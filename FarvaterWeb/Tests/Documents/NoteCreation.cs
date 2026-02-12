using FarvaterWeb.ApiServices;
using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{


    public class NoteCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private DocumentsPage Documents => new DocumentsPage(Page, Log, _test);

        private NotesPage Notes => new NotesPage(Page, Log, _test);

        private UserApiService UserApi => new UserApiService(ApiRequest);

        public NoteCreationTests(ITestOutputHelper output) : base(output) { }

        [Fact(DisplayName = "Проверка успешного создания записки")]

        public async Task ShouldCreateNote()
        {
            // Создание пользователя через API

            string postfix = DataPostfixExtensions.GetUniquePostfix();

            string lastName = "Тестерович";
            string firstName = "Андрей";
            string login = $"lastName{postfix}";

            string? userHandle = null;

            try

            {
                userHandle = await UserApi.PrepareUserAsync(lastName, firstName, login);


                Log.Information("--- Запуск сценария: Создание записки---");
                await LoginAsAdmin();
                await SideMenu.OpenSection("Записки", "notes");

                // Нажатие кнопки "Создание документа"

                await Notes.ClickCreateDocumentButton();


                // Ввод и выбор данных (Тип документа, Тема, Содержание, Адресаты, ). Проект пока выбирать не буду. Потом добавлю, когда разберусь с API

                var noteDetails = new NoteDetails
                    (
                    "Служебная",
                    "Тема служебной записки",
                    "Содержание служебной записки",
                    lastName
                     );

                await Notes.FillNoteDetails(noteDetails);

                // Нажатие кнопки "Отмена"

                await Notes.CancelAndVerify(noteDetails.Topic);


                // Нажатие кнопки "Создание документа"


                // Ввод и выбор данных (Тип документа, Тема, Содержание, Адресаты, ). Проект пока выбирать не буду. Потом добавлю, когда разберусь с API

                await Notes.FillNoteDetails(noteDetails);

                // Нажатие кнопки "Создать"

                await Notes.ClickCreateButton();

                // Удаление записки


                await Notes.DeleteCreatedNote(noteDetails.Topic);
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
