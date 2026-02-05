using FarvaterWeb.Base;
using FarvaterWeb.Data;
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

        public NoteCreationTests(ITestOutputHelper output)  : base(output) { }

        [Fact(DisplayName ="Проверка успешного создания записки")]

        public async Task ShouldCreateNote ()
        {
            // Создание контрагента через API

            // Создание Проекта через API
            Log.Information("--- Запуск сценария: Создание записки---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Записки", "notes");

            // Нажатие кнопки "Создание документа"

            // Выбор типа документа

            // Ввод и выбор данных (Тип документа, Тема, Содержание, Адресаты, ). Проект пока выбирать не буду. Потом добавлю, когда разберусь с API
                        
            // Нажатие кнопки "Отмена"

            // Нажатие кнопки "Создать

            // Удалить адресата из базы

            // Удалить проект из базы
        }

    }
}
