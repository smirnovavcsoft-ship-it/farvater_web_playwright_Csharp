using AventStack.ExtentReports;
using AventStack.ExtentReports.Model;
using FarvaterWeb.Components;
using Microsoft.Playwright;
using Serilog;
using System.Text.RegularExpressions;

namespace FarvaterWeb.Base;

public abstract class BasePage : BaseComponent
{
   
    // Конструктор просто пробрасывает зависимости в BaseComponent
    // Мы передаем null в качестве Root, так как у страницы нет родительского локатора
    public TableComponent Table => new TableComponent(Page, Log, _test);

    public CancelComponent CancelAction => new CancelComponent(Page, Log, _test);

    
    protected BasePage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test, "Page")
    {
        
    }   
    
    public async Task GoToUrl(string url, string expectedUrlPart)
    {
        // Оборачиваем всю навигацию в мастер-метод Do
        await Do($"[Navigation] Переход на URL: {url}", async () =>
        {
            // 1. Выполняем переход
            await Page.GotoAsync(url);

            // 2. Ждем загрузки DOM (LoadState.Load надежнее, чем NetworkIdle)
            await Page.WaitForLoadStateAsync(LoadState.Load);

            // 3. Проверяем, что мы попали на нужную страницу
            // Используем Assertions, чтобы при ошибке Allure и логи зафиксировали провал
            await Assertions.Expect(Page).ToHaveURLAsync(new Regex($".*{expectedUrlPart}.*"), new() { Timeout = 10000 });

            // 4. Опционально: делаем скриншот загруженной страницы
            await TakeScreenshotAsync($"Nav_{expectedUrlPart.Replace("/", "_")}");
        });
    }

    
    public async Task TakeScreenshotAsync(string name)
    {
        // 1. Убираем всё лишнее из имени (звездочки, двоеточия и т.д.)
        // string.Join объединяет разрешенные символы, отсекая запрещенные
        string safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
        safeName = safeName.Replace("*", ""); // На всякий случай удаляем звезду явно

        // 2. Формируем имя файла. ВНИМАНИЕ: используем safeName!
        // Важно: в дате используем дефисы, так как двоеточия тоже запрещены.
        string fileName = $"step_{safeName}_{DateTime.Now:HH-mm-ss}.png";

        // 3. Формируем путь
        string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults", "Screenshots");
        string fullPath = Path.Combine(folderPath, fileName);

        // 4. Логируем путь ПЕРЕД созданием, чтобы ты увидел его в консоли
        Log.Information("[Debug] Сохраняем файл в: {FullPath}", fullPath);

        await Do($"[Screenshot] Фиксация экрана: {name}", async () =>
        {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = fullPath });

            AllureService.AddAttachment(name, fullPath);
        });
    }

    /// <summary>
    /// Прокрутка всей страницы в самый низ (например, для подгрузки контента)
    /// </summary>
    public async Task ScrollToBottom()
    {
        // Оборачиваем в Do для логирования в отчеты
        await Do("[Page] Прокрутка страницы вниз", async () =>
        {
            await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");

            // Полезно сделать скриншот после прокрутки, 
            // чтобы видеть, что подгрузилось внизу
            await TakeScreenshotAsync("ScrollToBottom");
        });
    }

    /// <summary>
    /// Проверка заголовка вкладки браузера
    /// </summary>
    public async Task AssertPageTitle(string expectedTitle)
    {
        // Используем Do, так как это важная точка проверки (Assertion)
        await Do($"[Assertion] Проверка заголовка страницы: '{expectedTitle}'", async () =>
        {
            // Playwright будет ждать совпадения заголовка (по умолчанию 5с)
            await Assertions.Expect(Page).ToHaveTitleAsync(expectedTitle);
        });
    }
}