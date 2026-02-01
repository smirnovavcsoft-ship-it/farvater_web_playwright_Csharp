using FarvaterWeb.Tests.Counterparty;
using Microsoft.Playwright;
using FarvaterWeb.Base;
using Serilog;
using AventStack.ExtentReports;
using FarvaterWeb.Extensions;

namespace FarvaterWeb.Pages
{
    public class NewLegalPage : BasePage
    {

        private ILocator ContactsAccordion => Page.Locator("//div[@role='button'][.//span[text()='Контакты']]");

        

        public NewLegalPage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test) { }

        public async Task FillForm(LegalDetails details)
        {
            Log.Information("[NewLegalPage] Заполнение формы по заголовкам");

            await DoFillByLabel("ИНН", details.Inn);
            await DoFillByLabel("Краткое наименование", details.ShortName);
            await DoFillByLabel("Полное наименование", details.FullName);
            await DoFillByLabel("Адрес", details.Address);
            await DoFillByLabel("ОГРН", details.Ogrn);
            await DoFillByLabel("КПП", details.Kpp);
            await DoFillByLabel("Телефон", details.Phone);
            await DoFillByLabel("E-mail", details.Email);
        }

        public async Task Create()
        {
            await ButtonWithText("Создать").SafeClickAsync();
        }

        public async Task AddContact()
        {
            await ButtonWithText("Добавить контакт").SafeClickAsync();
        }

        
    }
}