using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;
using AventStack.ExtentReports;
using FarvaterWeb.Tests.Counterparty;

namespace FarvaterWeb.Pages
{
    public class CounterpartyPage : BasePage
    {
        private ILocator AddButton => Page.Locator("//button[.//span[text()='Добавить']]");

        private ILocator NewLegalOption => Page.Locator("//div[text()='Юр. лицо']");

        public CounterpartyPage(IPage page, Serilog.ILogger logger, ExtentTest test) : base(page, logger, test)
        {
        }

        public async Task SelectPersonTypeAsync()
        {
            await DoClick(AddButton, "Кнопка 'Добавить'");

            await DoClick(NewLegalOption, "Пункт меню 'Юр. лицо'");

            await Page.WaitForURLAsync("**/counterparty/newlegal**");
            Log.Information("[CounterpartyPage] Переход в форму создания нового Юр. лица выполнен");
        }

        public async Task Open()
        {
            await GoToUrl("https://farvater.mcad.dev/farvater/counterparty", "counterparty");
        }


        public async Task DeleteCounterparty(string name)
        {
            Log.Information("[CounterpartyPage] Удаление контрагента: {Name}", name);

            await GoToUrl("https://farvater.mcad.dev/farvater/counterparty", "counterparty");

            await AutoScreenshot($"Go_to_URL_{name}");

            var row = Page.Locator("tr").Filter(new() { HasText = name }).First;

            var deleteIcon = row.Locator("div[class*='menuItemDelete']");

            try
            {
                await row.ScrollIntoViewIfNeededAsync();

                await deleteIcon.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

                await deleteIcon.ClickAsync(new() { Force = true });

                await AutoScreenshot($"Confirm_Delete_Dialog_{name}");

                
                Log.Information("[CounterpartyPage] Нажата иконка удаления для {Name}", name);
                await AutoScreenshot($"Delete_Clicked_{name}");
            }
            catch (Exception ex)
            {
                Log.Error("[CounterpartyPage] Ошибка при удалении {Name}: {Error}", name, ex.Message);
                throw;
            }
        }
    }
}