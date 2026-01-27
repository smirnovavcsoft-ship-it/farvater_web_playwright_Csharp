using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Tests.Documents
{
    [Collection("AllureCollection")]
    public class IncomeDocumentCreationTests : BaseTest
    {
        public IncomeDocumentCreationTests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact(DisplayName ="Проверка успешного создания входящего документа")]
        public async Task SouldCreateIncomeDocument ()
        {
            Log.Information("--- Запуск сценария: Создание входящего документа---");
            await LoginAsAdmin();
            await SideMenu.OpenSelection("Входящие", "income");

            // Клик по кнопке "Создать документ"

            // Выбор типа документа

            // Заполнение полей документа

            var incomeDocumentsDetails = new IncomeDocumentDetails("Краткое содержание, краткое содержание", "14645");

            // Нажатие кнопки "Отмена"

            // Клик по кнопке "Создать документ"

            // Выбор типа документа

            // Выбор планируемой даты ответа

            // Заполнение полей документа

            // Выбор проекта в выпадающем списке "Проект"

            // Выбор отправителья в выпадающем списке "Отправитель*"

            // Выбор даты "От"

            // Выбор "От отправителья подписал"

            // Выбор исполнителя

            // Нажатие кнопки "Отмена"

            // Нажатие кнопки "Создать"

            // Открытие и закрытие документа в списке

            // Удаление документа
        }

    }
}
