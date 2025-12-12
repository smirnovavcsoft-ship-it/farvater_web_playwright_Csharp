using Microsoft.Playwright;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright.Core; // Может потребоваться для некоторых расширений
using System; // Для исключений




namespace FarvaterWeb.Pages.Common
{
    // Базовый класс для всех Page Objects
    public abstract class BasePage
    {
        // Приватное поле для хранения объекта Page
        protected IPage Page { get; }

        // Поля для учетных данных (если используются)
        // В C# часто используют статический класс для хранения конфига
        // Здесь для примера используем System.Environment
        protected string Username => Environment.GetEnvironmentVariable("USER_LOGIN");
        protected string Password => Environment.GetEnvironmentVariable("PASSWORD");

        // Счетчик скриншотов
        private int _screenshotCounter = 0;

        // В конструктор всегда передается объект IPage
        public BasePage(IPage page, string baseUrl)
        {
            Page = page;
        }

        // --- НАВИГАЦИЯ ---

        public async Task GoToUrl(string url, string expectedUrlPart, float timeout = 15000)
        {
            // 1. Выполняем навигацию
            // Playwright for .NET использует метод GoToAsync
            await Page.GotoAsync(url);
            Console.WriteLine($"Начата загрузка страницы: {url}");

            // 2. Ожидаем, что URL содержит ожидаемую часть
            // Это является надежным подтверждением успешной навигации и загрузки
            Console.WriteLine($"[Ожидание] Ждем URL, содержащий '{expectedUrlPart}' (Timeout: {timeout / 1000}s)...");

            try
            {
                // Используем Page.WaitForURLAsync для ожидания конкретной части URL.
                // **{expectedUrlPart}** позволяет искать подстроку.
                await Page.WaitForURLAsync($"**{expectedUrlPart}**", new PageWaitForURLOptions { Timeout = timeout });

                string currentUrl = Page.Url;

                // 3. Логирование успешного перехода
                Console.WriteLine($"[Навигация] УСПЕХ. Открыта страница: {currentUrl}");
            }
            catch (TimeoutException)
            {
                // 4. Обработка ошибки
                string currentUrl = Page.Url;
                throw new Exception($"Не удалось открыть страницу. URL не содержит ожидаемую часть '{expectedUrlPart}' в течение {timeout / 1000}s. Текущий URL: {currentUrl}");
            }

            // 5. Снимаем скриншот после успешного перехода
            await TakeScreenshotAsync($"GoTo_{expectedUrlPart.Replace("/", "_")}");
        }

        // --- ДЕЙСТВИЯ С ЭЛЕМЕНТАМИ ---

        /**
         * Кликает по элементу, используя Playwright locator.
         */
        /*public async Task ClickAsync(string locator, string actionDescription = "ClickElement")
        {
            // Page.Locator() создает локатор, ClickAsync() выполняет клик
            await Page.Locator(locator).ClickAsync();
            Console.WriteLine($"Клик по элементу: {actionDescription}");
            await TakeScreenshotAsync(actionDescription);
        }*/

        public async Task ClickAsync(string locator,
                             string actionDescription = "ClickElement",
                             string expectedUrlPart = null, // Новый необязательный параметр
                             float timeout = 15000)        // Опциональный тайм-аут
        {
            // 1. Сохраняем текущий URL ДО клика.
            string urlBeforeClick = Page.Url;

            // 2. Выполняем чистое действие Playwright (преобразование строки в локатор и клик)
            await Page.Locator(locator).ClickAsync();

            // 3. Логируем само действие клика
            Console.WriteLine($"Клик по элементу: {actionDescription}");

            // --- НАЧАЛО БЛОКА ПРОВЕРКИ URL ---

            // ПРОВЕРКА 1: Если ожидается переход на конкретный URL (expectedUrlPart НЕ null/пустой)
            if (!string.IsNullOrEmpty(expectedUrlPart))
            {
                Console.WriteLine($"[Ожидание] Ждем URL, содержащий '{expectedUrlPart}' (Timeout: {timeout / 1000}s)...");

                try
                {
                    // Ожидаем, что URL изменится и будет содержать ожидаемую часть
                    await Page.WaitForURLAsync($"**{expectedUrlPart}**", new PageWaitForURLOptions { Timeout = timeout });

                    string currentUrl = Page.Url;
                    Console.WriteLine($"[Навигация] УСПЕХ. URL содержит '{expectedUrlPart}'. Текущий URL: {currentUrl}");
                }
                catch (TimeoutException)
                {
                    // Если ожидание по тайм-ауту не удалось
                    string currentUrl = Page.Url;
                    throw new Exception($"URL не содержит ожидаемую часть '{expectedUrlPart}' в течение {timeout / 1000}s. Текущий URL: {currentUrl}");
                }
            }
            // ПРОВЕРКА 2: Если ожидаемой части URL нет, но URL просто изменился (существующая логика)
            else
            {
                // Получаем URL ПОСЛЕ клика.
                string urlAfterClick = Page.Url;

                if (urlBeforeClick != urlAfterClick)
                {
                    Console.WriteLine($"[Навигация] Успешный переход (неожиданный/общее изменение). Новый URL: {urlAfterClick}");
                }
            }

            // --- КОНЕЦ БЛОКА ПРОВЕРКИ URL ---

            // 4. Снимаем скриншот
            await TakeScreenshotAsync(actionDescription);
        }

        /**
         * Вводит текст в поле. Playwright .FillAsync() автоматически очищает поле.
         */
        public async Task FillFieldAsync(string locator, string text, string actionDescription = "TypeElement")
        {
            // 1. Ввод текста. Playwright .FillAsync() автоматически очищает поле.
            await Page.Locator(locator).FillAsync(text);

            // 2. Проверка: получаем текущее значение поля по тому же локатору
            // InputValueAsync() надежно возвращает текст, который видит Playwright в поле.
            string actualValue = await Page.Locator(locator).InputValueAsync();

            // 3. Сравнение
            if (actualValue != text)
            {
                // Если значения не совпадают, выбрасываем исключение
                throw new Exception($"Ошибка ввода в поле: {actionDescription}. Ожидалось: '{text}', Фактически: '{actualValue}'");
            }

            // 4. Логирование (если проверка прошла успешно)
            Console.WriteLine($"Введено '{text}' в элемент: {actionDescription}");
            Console.WriteLine($"[Проверка] Успешная валидация значения: '{actualValue}'");

            // 5. Скриншот
            await TakeScreenshotAsync(actionDescription);
        }

        // --- ПРОВЕРКИ ---

        public async Task<bool> IsUrlContainsAsync(string expectedUrlPart, float timeout = 15000)
        {
            // В C# можно использовать встроенные методы Expect, но
            // для ToHaveURL используется расширение PlaywrightTest (XUnit/NUnit)

            // Здесь используем более чистую проверку через Page.Url
            await Page.WaitForURLAsync($"**{expectedUrlPart}**", new PageWaitForURLOptions { Timeout = timeout });

            string currentUrl = Page.Url;
            if (!currentUrl.Contains(expectedUrlPart))
            {
                throw new Exception($"URL не содержит '{expectedUrlPart}'. Текущий URL: {currentUrl}");
            }

            Console.WriteLine($"URL содержит '{expectedUrlPart}'. Текущий URL: {currentUrl}");
            await TakeScreenshotAsync("CheckUrl");
            return true;
        }

        public async Task WaitForElementToStaleAsync(string locator, float timeout = 5000)
        {
            // Ожидаем, что элемент будет скрыт или удален
            // ИСПРАВЛЕНО: Теперь это один объект LocatorWaitForOptions
            /* await Page.Locator(locator).WaitForAsync(
                 new LocatorWaitForOptions
                 {
                     State = WaitForState.Hidden, // <--- Обе настройки находятся ВНУТРИ объекта
                     Timeout = timeout
                 }
             );*/
            await Page.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions
            {
                // Ожидаем, что элемент будет скрыт
                State = Microsoft.Playwright.WaitForSelectorState.Hidden,
                Timeout = timeout
            });

            Console.WriteLine("Элемент исчез со страницы.");
        }

        // --- ПОЛУЧЕНИЕ ЗНАЧЕНИЙ И СОСТОЯНИЯ ---

        public async Task<string> GetInputValueAsync(string locator)
        {
            return await Page.Locator(locator).InputValueAsync();
        }

        public async Task<bool> IsElementVisibleAsync(string locator)
        {
            // IsVisibleAsync() возвращает bool
            return await Page.Locator(locator).IsVisibleAsync();
        }

        // --- СКРОЛЛИНГ И ДЕЙСТВИЯ С ВЫПАДАЮЩИМИ СПИСКАМИ ---

        public async Task ScrollElementIntoViewAsync(string locator)
        {
            // Playwright автоматически прокручивает, но можно вызвать явно
            await Page.Locator(locator).ScrollIntoViewIfNeededAsync();
            Console.WriteLine("Элемент прокручен в видимую область.");
        }

        /**
         * Выбирает опцию из стандартного HTML-тега <select> по видимому тексту.
         */
        public async Task SelectOptionByTextAsync(string selectLocator, string optionText, string actionDescription = "SelectOption")
        {
            // В C# selectOption заменяется на SelectOptionAsync. 
            // Используем SelectOptionValue, чтобы выбрать по видимому тексту
            await Page.Locator(selectLocator).SelectOptionAsync(new SelectOptionValue { Label = optionText });

            Console.WriteLine($"В выпадающем списке {selectLocator} выбрана опция: {optionText}");
            await TakeScreenshotAsync($"{actionDescription}_{optionText.Replace(" ", "_")}");
        }

        // --- СКРИНШОТЫ ---
        /*
        public async Task TakeScreenshotAsync(string fileName)
        {
            try
            {
                _screenshotCounter++;
                string paddedCounter = _screenshotCounter.ToString().PadLeft(2, '0');

                // Заменяем недопустимые символы
                string safeFileName = fileName
                    .ReplaceAny(Path.GetInvalidFileNameChars(), '_')
                    .Replace(" ", "_");

                string finalFileName = $"{paddedCounter}_{safeFileName}.png";

                // Используем Path.Combine для создания корректного пути
                string screenshotDir = Path.Combine(Directory.GetCurrentDirectory(), "screenshots");

                if (!Directory.Exists(screenshotDir))
                {
                    Directory.CreateDirectory(screenshotDir);
                }

                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = Path.Combine(screenshotDir, finalFileName),
                    FullPage = true
                });
                Console.WriteLine($"Скриншот сохранён: {finalFileName}");
            }
            catch (Exception error)
            {
                Console.WriteLine($"Ошибка при сохранении скриншота: {error.Message}");
            }
        }*/

        // --- СКРИНШОТЫ ---
        public async Task TakeScreenshotAsync(string fileName)
        {
            try
            {
                _screenshotCounter++;
                string paddedCounter = _screenshotCounter.ToString().PadLeft(2, '0');

                // Получаем список недопустимых символов и заменяем их
                string safeFileName = fileName;
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    safeFileName = safeFileName.Replace(c, '_');
                }
                // Заменяем пробелы на подчеркивание
                safeFileName = safeFileName.Replace(' ', '_');

                string finalFileName = $"{paddedCounter}_{safeFileName}.png";

                string screenshotDir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "screenshots");

                if (!Directory.Exists(screenshotDir))
                {
                    Directory.CreateDirectory(screenshotDir);
                }

                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = System.IO.Path.Combine(screenshotDir, finalFileName),
                    FullPage = true
                });
                Console.WriteLine($"Скриншот сохранён: {finalFileName}");
            }
            catch (Exception error)
            {
                Console.WriteLine($"Ошибка при сохранении скриншота: {error.Message}");
            }
        }
    }
}

// ** Примечание: Для корректной работы метода IsUrlContainsAsync
// и метода TakeScreenshotAsync (при использовании Path.ReplaceAny)
// могут потребоваться дополнительные using'и или вспомогательные методы.
// В рабочем проекте C# Playwright часто используется пакет Microsoft.Playwright.MSTest
// или NUnit, который добавляет удобные Assert'ы.