using FarvaterWeb.Tests.Counterparty;
using Microsoft.Playwright;
using FarvaterWeb.Base;
using Serilog;
using AventStack.ExtentReports;

namespace FarvaterWeb.Pages
{
    public class NewLegalPage : BasePage
    {
        // Локаторы через name аттрибуты (более стабильно чем XPath)
        private ILocator InnInput => Page.Locator("input[name='inn']");
        private ILocator ShortNameInput => Page.Locator("input[name='shorttitle']");
        private ILocator FullNameInput => Page.Locator("input[name='title']");
        private ILocator SaveButton => Page.Locator("//button[.//span[text()='Сохранить']]");
        private ILocator ContactsAccordion => Page.Locator("//div[@role='button'][.//span[text()='Контакты']]");

        public NewLegalPage(IPage page, Serilog.ILogger logger, ExtentTest extentTest) : base(page, logger, extentTest) { }

        public async Task FillForm(LegalDetails details)
        {
            Log.Information("[NewLegalPage] Заполнение формы нового юр. лица");

            await DoFill(InnInput, "ИНН", details.Inn);
            await DoFill(ShortNameInput, "Краткое имя", details.ShortName);
            await DoFill(FullNameInput, "Полное имя", details.FullName);
            // ... остальные поля через DoFill ...
        }

        public async Task Save()
        {
            await DoClick(SaveButton, "Кнопка 'Сохранить'");
            // Здесь можно добавить ожидание исчезновения формы или появления уведомления
        }
    }
}