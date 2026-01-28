using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{
    [Collection("AllureCollection")]
    public class IncomeDocumentCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private DocumentsPage Documents => new DocumentsPage(Page, Log, _test);
        public IncomeDocumentCreationTests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact(DisplayName ="Проверка успешного создания входящего документа")]
        public async Task SouldCreateIncomeDocument ()
        {
            Log.Information("--- Запуск сценария: Создание входящего документа---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Входящие", "income");

            // Клик по кнопке "Создать документ"

            await Documents.ClickCreateDocumentButton();

            // Выбор типа документа

            

            await Documents.SelectDocumentType();

            // Заполнение полей документа

            var incomeDocumentsDetails = new IncomeDocumentDetails("Краткое содержание, краткое содержание", "14645");

            await Documents.FillIncomeDocumetDetails(incomeDocumentsDetails);

            // Нажатие кнопки "Отмена"

            await Documents.ClickCancelButton();

            // Клик по кнопке "Создать документ"

            await Documents.ClickCreateDocumentButton();

            // Выбор типа документа

            await Documents.SelectDocumentType();

            // Выбор планируемой даты ответа



            // Заполнение полей документа

            await Documents.FillIncomeDocumetDetails(incomeDocumentsDetails);

            // Выбор проекта в выпадающем списке "Проект"

            await Documents.SelectProject();

            // Выбор отправителья в выпадающем списке "Отправитель*"

            await Documents.SelectSender();

            // Выбор даты "От"



            // Выбор "От отправителья подписал"



            // Выбор исполнителя

            

            // Нажатие кнопки "Создать"

            await Documents.ClickCreateButton();

            // Открытие и закрытие документа в списке



            // Удаление документа
        }

    }
}
