using FarvaterWeb.Tests.Counterparty;
using Microsoft.Playwright;
using FarvaterWeb.Base;
using Serilog;
using AventStack.ExtentReports;

namespace FarvaterWeb.Pages
{
    public class NewLegalPage : BaseComponent
    {
        // Локаторы через name аттрибуты (более стабильно чем XPath)
        /*private ILocator InnInput => Page.Locator("input[name='inn']");
        private ILocator ShortNameInput => Page.Locator("input[name='shorttitle']");
        private ILocator FullNameInput => Page.Locator("input[name='title']");
        private ILocator AddressInput => Page.Locator("input[name='address'");
        private ILocator OGRNInput => Page.Locator("input[name='ogrn'");
        private ILocator KPPInput => Page.Locator("input[name='kpp'");
        private ILocator PhoneInput => Page.Locator("input[name='phone'");
        private ILocator EmailInput => Page.Locator("input[name='email'");*/


        //private ILocator SaveButton => Page.Locator("//button[.//span[text()='Сохранить']]");
        private ILocator ContactsAccordion => Page.Locator("//div[@role='button'][.//span[text()='Контакты']]");

        

        public NewLegalPage(IPage page, ILogger logger, ExtentTest extentTest) : base(page, logger, extentTest) { }

        /*public async Task FillForm(LegalDetails details)
        {
            Log.Information("[NewLegalPage] Заполнение формы нового юр. лица");

            await DoFill(InnInput, "ИНН", details.Inn);
            await DoFill(ShortNameInput, "Краткое имя", details.ShortName);
            await DoFill(FullNameInput, "Полное имя", details.FullName);
            await DoFill(AddressInput, "Адрес", details.Address);
            await DoFill(OGRNInput, "ОГРН", details.Ogrn);
            await DoFill(KPPInput, "КПП", details.Kpp);
            await DoFill(PhoneInput, "Телефон", details.Phone);
            await DoFill(EmailInput, "Email", details.Email);
           
        }*/

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

        /*public async Task Save()
        {
            await DoClick(SaveButton, "Кнопка 'Сохранить'");
            // Здесь можно добавить ожидание исчезновения формы или появления уведомления
        }*/

        public async Task Create()
        {
            await DoClickByText("Создать");
        }

        public async Task AddContact()
        {
            await DoClickByText("Добавить контакт");
        }

        
    }
}