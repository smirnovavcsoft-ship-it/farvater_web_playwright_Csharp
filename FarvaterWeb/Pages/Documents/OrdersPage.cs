using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.Pages.Documents
{
    public class OrdersPage : BaseComponent
    {
        private SmartLocator CreateDocumentButton => Button.WithText("Создать документ");
        private SmartLocator DocumentTypeDropdown => Dropdown.WithLabel("Тип документа *");

        private SmartLocator OrderNameInput => Input.WithLabel("Приказ *");

        private SmartLocator OrderContentDescription => Description.WithPlacholder("Введите текст документа");

        private SmartLocator SignedDropdown => Dropdown.WithLabel("Подписал");

        private DateComponent Date(string label) => new(Page, Log, _test, label, GetType().Name);

        private SmartLocator CancelButton => Button.WithText("Отмена");

        private SmartLocator CreateButton => Button.WithText("Создать");

        public OrdersPage(IPage page, ILogger logger, ExtentTest _test) : base(page, logger, _test) { }

        public async Task ClickCreateDocmentButton()
        {
            await CreateDocumentButton.SafeClickAsync();
        }

        public async Task FillOrderDetails(OrderDetails details)
        {
            await DocumentTypeDropdown.SelectByTextAndVerifyAsync(details.DocumentType);
            await OrderNameInput.ClearAndFillAsync(details.OrderName);
            await OrderContentDescription.ClearAndFillAsync(details.OrderContent);
            await SignedDropdown.SelectUserAndVerifyAsync(details.LastName, details.FirstName);
            await Date("Дата подписания").SetTodayAsync();
        }




    }
}
