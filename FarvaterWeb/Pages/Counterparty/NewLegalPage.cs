using Microsoft.Playwright;
using System.Threading.Tasks;
using FarvaterWeb.Pages.Common; // Убедитесь, что здесь указано пространство имен для BasePage
using FarvaterWeb.Tests.Counterparty; // Используем record LegalDetails из тестового файла

namespace FarvaterWeb.Pages
{
    public class NewLegalPage : BasePage
    {
        // 1. Определение относительного пути
        private const string RelativePath = "counterparty/newlegal";

        // 2. Инкапсуляция Локаторов (как свойства ILocator)
        
        // Поля ввода
        private ILocator InnInput => Page.Locator("//input[@name=\"inn\"]");
        private ILocator ShortNameInput => Page.Locator("//input[@name=\"shorttitle\"]");
        private ILocator FullNameInput => Page.Locator("//input[@name=\"title\"]");
        private ILocator AddressInput => Page.Locator("//input[@name=\"address\"]");
        private ILocator OgrnInput => Page.Locator("//input[@name=\"ogrn\"]");
        private ILocator KppInput => Page.Locator("//input[@name=\"kpp\"]");
        private ILocator PhoneInput => Page.Locator("//input[@name=\"phone\"]");
        private ILocator EmailInput => Page.Locator("//input[@name=\"email\"]");

        // Кнопки и блоки
        private ILocator ContactsBlockDropDownButton => Page.Locator("//div[@role=\"button\" and .//span[text()=\"Контакты\"]]");
        private ILocator AddContactButton => Page.Locator("//button[.//span[text()=\"Добавить контакт\"]]");
        private ILocator SaveButton => Page.Locator("//button[.//span[text()=\"Сохранить\"]]");


        // 3. Конструктор
        public NewLegalPage(IPage page, string baseUrl, string username, string password) : base(page, baseUrl, username, password)
        {
        }
        
        // --- 4. Методы действий ---

        /// <summary>
        /// Заполняет основные текстовые поля контрагента.
        /// </summary>
        /// <param name="detailsObject">Объект LegalDetails с данными.</param>
        public async Task FillNewLegalDetailsAsync(LegalDetails detailsObject)
        {
            // Здесь мы используем встроенный метод FillAsync, который работает быстрее, 
            // чем имитация fillField из JS.
            await InnInput.FillAsync(detailsObject.Inn);
            Console.WriteLine("Ввод ИНН"); // Логирование заменено на Console.WriteLine для простоты
            
            await ShortNameInput.FillAsync(detailsObject.ShortName);
            Console.WriteLine("Ввод краткого наименования");
            
            await FullNameInput.FillAsync(detailsObject.FullName);
            Console.WriteLine("Ввод полного наименования");
            
            await AddressInput.FillAsync(detailsObject.Address);
            Console.WriteLine("Ввод адреса");
            
            await OgrnInput.FillAsync(detailsObject.Ogrn);
            Console.WriteLine("Ввод ОГРН");
            
            await KppInput.FillAsync(detailsObject.Kpp);
            Console.WriteLine("Ввод КПП");
            
            await PhoneInput.FillAsync(detailsObject.Phone);
            Console.WriteLine("Ввод телефона");
            
            await EmailInput.FillAsync(detailsObject.Email);
            Console.WriteLine("Ввод электронной почты");
        }

        /// <summary>
        /// Получает значения из полей для проверки.
        /// </summary>
        public async Task<LegalDetails> GetNewLegalDetailsAsync()
        {
            // Используем метод ILocator.InputValueAsync() для получения значения из полей
            return new LegalDetails(
                Inn: await InnInput.InputValueAsync(),
                ShortName: await ShortNameInput.InputValueAsync(),
                FullName: await FullNameInput.InputValueAsync(),
                Address: await AddressInput.InputValueAsync(),
                Ogrn: await OgrnInput.InputValueAsync(),
                Kpp: await KppInput.InputValueAsync(),
                Phone: await PhoneInput.InputValueAsync(),
                Email: await EmailInput.InputValueAsync()
            );
        }

        /// <summary>
        /// Раскрывает блок "Контакты" и проверяет видимость кнопки "Добавить контакт".
        /// </summary>
        public async Task ClickContactsBlockDropdownButtonAsync()
        {
            await ContactsBlockDropDownButton.ClickAsync();
            Console.WriteLine("Выпадающий список \"Контакты\"");
            
            // В C# Playwright ILocator.IsVisibleAsync() вернет bool.
            if (await AddContactButton.IsVisibleAsync())
            {
                 Console.WriteLine("Блок \"Контакты\" успешно отображен на странице.");
            }
        }

        public async Task ClickAddContactButtonAsync()
        {
            await AddContactButton.ClickAsync();
            Console.WriteLine("Кнопка \"Добавить контакт\" успешно нажата.");
        }

        public async Task ClickSaveButtonAsync()
        {
            await SaveButton.ClickAsync();
            Console.WriteLine("Кнопка \"Сохранить\" успешно нажата.");
        }

        /// <summary>
        /// Ожидает полной загрузки формы, проверяя видимость поля ИНН.
        /// </summary>
        public async Task WaitForFormToLoadAsync()
        {
            // Playwright ILocator.ToString() дает строку селектора, которую может использовать WaitForSelectorAsync.
            await Page.WaitForSelectorAsync(InnInput.ToString(), new PageWaitForSelectorOptions
            {
                // Используем полное имя для обхода конфликтов/проблем со ссылками
                State = Microsoft.Playwright.WaitForSelectorState.Visible,
                Timeout = 15000
            });

            Console.WriteLine("Форма создания юр. лица успешно загружена.");
        }
    }
}