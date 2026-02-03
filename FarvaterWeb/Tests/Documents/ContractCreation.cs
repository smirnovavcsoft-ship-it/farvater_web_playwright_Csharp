using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{
    public class ContractCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private DocumentsPage Documents => new DocumentsPage(Page, Log, _test);

        public ContractCreationTests (ITestOutputHelper output) : base(output) { }

        [Fact(DisplayName ="Проверка успешного создания договора")]

        public async Task ShouldCreateContract ()
        {
            Log.Information("--- Запуск сценария: Создание договора---");
            await LoginAsAdmin();
            await SideMenu.OpenSection("Договоры", "contracts");

            // Клик по кнопке "Создать договор"

           await Documents.ClickCreateContractButton();

            // Заполнение полей ввода ("Предмет договора", "Стоимость", "В том числе НДС", "Полная стоимость", "Именуемая в дальнейшем", "Именуемая в дальнейшем")

            var contractDetails = new ContractDetails
                (
                "Разработка документации",
                "Сторона 1",
                "Сторона 2",
                "12464",
                "145465",
                "514416541"
                );

            // await Documents.FillContractDetails( contractDetails );

            // Выбор типа договора

            await Documents.SelectContractType();

            // Выбор стороны 1 (нет плюсцов для создания контрагента прямо из формы создания договора, сложно будет создавать контагента)

            await Documents.SelectParty1();

            // Выбор стороны 2 (нет плюсцов для создания контрагента прямо из формы создания договора, сложно будет создавать контагента)

            await Documents.SelectParty2();

            // Назначение сроков по договору

            DateTime startDate = new DateTime(2026, 02, 03);

            DateTime endDate = new DateTime(2026, 03, 03);

            await Documents.AppointContractTerm(startDate, endDate);

            // Нажатие кнопки "Отмена"

            // Клик по кнопке "Создать договор"

            // Заполнение поля "Предмет договора"

            // Выбор типа договора

            // Выбор стороны 1

            // Выбор стороны 2

            // Назначение сроков по договору

            // Заполнение полей ввода ("Предмет договора", "Стоимость", "В том числе НДС", "Полная стоимость", "Именуемая в дальнейшем", "Именуемая в дальнейшем")

            // Нажатие кнопки "Создать"




        }

    }
}
