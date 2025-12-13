using FarvaterWeb.Pages.Common;
using Microsoft.Playwright;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Playwright.Core; // Может потребоваться для некоторых расширений
using System; // Для исключений

namespace FarvaterWeb.Pages
{
    // Класс CounterpartyPage наследуется от нашего BasePage (который мы ранее обсуждали)
    public class CounterpartyPage : BasePage
    {
        // 1. Определение относительного пути (если нужно)
        private const string RelativePath = "counterparty";

        // 2. Инкапсуляция Локаторов (как свойства ILocator)

        // Локатор для кнопки "Добавить" (используем GetByRole для лучшей устойчивости, если возможно)
        // В данном случае, используем CSS или XPath, чтобы сохранить исходный локатор
        private ILocator AddButtonLocator => Page.Locator("//button[.//span[text()=\"Добавить\"]]");

        // Локатор для триггера выпадающего списка "Юр. лицо"
        private ILocator NewLegalDropdownTrigger => Page.Locator("//div[text()=\"Юр. лицо\"]");

        // 3. Конструктор
        // Принимает IPage и BaseUrl, передавая их в базовый класс
        public CounterpartyPage(IPage page, string baseUrl, string username, string password) : base(page, baseUrl, username, password)
        {
            // Здесь можно добавить дополнительную инициализацию, если требуется
        }

        // 4. Методы действий

        /// <summary>
        /// Выполняет клик по кнопке "Добавить" и затем выбирает "Юр. лицо".
        /// </summary>
        public async Task SelectPersonTypeAsync()
        {
            // Используем метод ClickAsync() на ILocator
            await AddButtonLocator.ClickAsync();

            // В C# для логирования лучше использовать ILogger, но для простоты оставим Console.WriteLine
            Console.WriteLine("Кнопка \"Добавить\" успешно нажата.");

            await NewLegalDropdownTrigger.ClickAsync();
            Console.WriteLine("Выбран пункт \"Юр. лицо\".");
        }

        /// <summary>
        /// Переходит на страницу контрагентов.
        /// </summary>
        public async Task GotoCounterpartyPage()
        {
            await GoToUrl(RelativePath, RelativePath);
        }
    }
}