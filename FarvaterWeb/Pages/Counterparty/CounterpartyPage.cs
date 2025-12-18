using FarvaterWeb.Base;
using Microsoft.Playwright;
using FarvaterWeb.Base;
using Serilog;

namespace FarvaterWeb.Pages
{
    public class CounterpartyPage : BasePage
    {
        // 1. Локаторы как свойства ILocator (ленивая инициализация)
        // Кнопка "Добавить"
        private ILocator AddButton => Page.Locator("//button[.//span[text()='Добавить']]");

        // Пункт "Юр. лицо" в выпадающем списке
        private ILocator NewLegalOption => Page.Locator("//div[text()='Юр. лицо']");

        // 2. Конструктор (теперь только Page и Logger)
        public CounterpartyPage(IPage page, Serilog.ILogger logger) : base(page, logger)
        {
        }

        /// <summary>
        /// Переход на страницу создания Юр. Лица через кнопку "Добавить"
        /// </summary>
        public async Task SelectPersonTypeAsync()
        {
            // Используем DoClick из BaseComponent (авто-логирование и стабильность)
            await DoClick(AddButton, "Кнопка 'Добавить'");

            // Выбираем тип контрагента
            await DoClick(NewLegalOption, "Пункт меню 'Юр. лицо'");

            // Ждем перехода на страницу создания, чтобы тест не бежал вперед паровоза
            await Page.WaitForURLAsync("**/counterparty/newlegal**");
            Log.Information("[CounterpartyPage] Переход в форму создания нового Юр. лица выполнен");
        }

        /// <summary>
        /// Прямой переход на страницу контрагентов (если нужно для теста)
        /// </summary>
        public async Task Open()
        {
            await GoToUrl("https://farvater.mcad.dev/farvater/counterparty", "counterparty");
        }
    }
}