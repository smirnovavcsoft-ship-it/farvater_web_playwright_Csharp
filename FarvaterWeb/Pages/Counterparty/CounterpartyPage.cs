using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;
using AventStack.ExtentReports;
using FarvaterWeb.Tests.Counterparty;

namespace FarvaterWeb.Pages
{
    public class CounterpartyPage : BaseComponent
    {
        // 1. Локаторы как свойства ILocator (ленивая инициализация)
        // Кнопка "Добавить"
        private ILocator AddButton => Page.Locator("//button[.//span[text()='Добавить']]");

        // Пункт "Юр. лицо" в выпадающем списке
        private ILocator NewLegalOption => Page.Locator("//div[text()='Юр. лицо']");

        // 2. Конструктор (теперь только Page и Logger)
        public CounterpartyPage(IPage page, Serilog.ILogger logger, ExtentTest extentTest) : base(page, logger, extentTest)
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

        /*public async Task DeleteCounterparty(string name)
        {
            Log.Information("[CounterpartyPage] Удаление контрагента: {Name}", name);

            // 1. Находим строку с контрагентом и нажимаем на кнопку действий или корзину рядом
            // Используем наш GetByRole или XPath, чтобы найти кнопку именно в этой строке
            await GoToUrl("https://farvater.mcad.dev/farvater/counterparty", "counterparty");
            var row = Page.Locator("tr").Filter(new() { HasText = name });
            await row.GetByRole(AriaRole.Button, new() { Name = "Удалить" }).ClickAsync();

            // 2. Подтверждаем удаление в системном диалоге или модальном окне
            await DoClickByText("Ok"); // Или "Подтвердить"
        }*/

        /*public async Task DeleteCounterpartyByName(string name)
        {
            Log.Information("[CounterpartyPage] Попытка удаления контрагента: {Name}", name);

            await GoToUrl("https://farvater.mcad.dev/farvater/counterparty", "counterparty");

            // 1. Находим строку, которая содержит имя контрагента
            // Используем фильтр, чтобы убедиться, что мы работаем с нужной строкой таблицы
            var row = Page.Locator("tr").Filter(new() { HasText = name }).First;

            try
            {
                // 2. Прокрутка к строке, если она внизу списка
                await row.ScrollIntoViewIfNeededAsync(new() { Timeout = 3000 });

                // 3. Ждем, когда строка станет видимой
                await Assertions.Expect(row).ToBeVisibleAsync(new() { Timeout = 5000 });

                // 4. Находим кнопку удаления ВНУТРИ этой строки
                // Если это иконка, используем GetByRole или GetByTitle
                var deleteButton = row.GetByRole(AriaRole.Button).Filter(new() { HasText = "Удалить" }).Or(row.Locator(".btn-delete")).First;

                await deleteButton.ClickAsync(new() { Force = true });

                // 5. Подтверждение в модальном окне (используем наш универсальный метод)
                await DoClickByText("Да");

                Log.Information("[CounterpartyPage] Контрагент {Name} успешно удален", name);
                await AutoScreenshot($"Deleted_{name}");
            }
            catch (Exception ex)
            {
                Log.Error("[CounterpartyPage] Не удалось удалить контрагента {Name}: {Error}", name, ex.Message);
                throw;
            }
        }*/


        public async Task DeleteCounterparty(string name)
        {
            Log.Information("[CounterpartyPage] Удаление контрагента: {Name}", name);

            await GoToUrl("https://farvater.mcad.dev/farvater/counterparty", "counterparty");

            await AutoScreenshot($"Go_to_URL_{name}");

            // 1. Ищем строку, в которой есть текст с названием компании
            var row = Page.Locator("tr").Filter(new() { HasText = name }).First;

            // 2. Внутри этой строки ищем иконку корзины по специфичному классу
            // Используем селектор, который ищет часть имени класса 'Delete'
            var deleteIcon = row.Locator("div[class*='menuItemDelete']");

            try
            {
                // 3. Скроллим к строке
                await row.ScrollIntoViewIfNeededAsync();

                // 4. Ждем кликабельности иконки
                await deleteIcon.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

                // 5. Кликаем
                await deleteIcon.ClickAsync(new() { Force = true });

                await AutoScreenshot($"Confirm_Delete_Dialog_{name}");

                
                // 6. Подтверждаем в модальном окне (если оно появляется)
                // Если после клика на корзину всплывает "Вы уверены?", раскомментируйте:
                await DoClickByText("Удалить"); 

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