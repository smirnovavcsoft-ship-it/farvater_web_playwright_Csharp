using AventStack.ExtentReports;
using AventStack.ExtentReports.Model;
using FarvaterWeb.Components;
using Microsoft.Playwright;
using Serilog;
using System.Text.RegularExpressions;

namespace FarvaterWeb.Base;

public abstract class BasePage : BaseComponent
{
    //protected readonly IPage Page;
   // protected readonly ILogger Log;
    //protected readonly ExtentTest _test;
    // Конструктор просто пробрасывает зависимости в BaseComponent
    // Мы передаем null в качестве Root, так как у страницы нет родительского локатора
    public TableComponent Table => new TableComponent(Page, Log, _test);

    public CancelComponent CancelAction => new CancelComponent(Page, Log, _test);

    
    protected BasePage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test, "Page")
    {
        
    }
    

    /// <summary>
    /// Переход по URL с проверкой результата и логированием
    /// </summary>
    /*public async Task GoToUrl(string url, string expectedUrlPart)
    {
        Log.Information("[Navigation] Переход на URL: {Url}. Ожидаем часть пути: '{Part}'", url, expectedUrlPart);

        try
        {
            // Переходим на страницу
            var response = await Page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

            // Проверяем, не ответил ли сервер ошибкой
            if (response != null && !response.Ok)
            {
                Log.Warning("[Navigation] Сервер вернул статус: {Status}", response.Status);
            }

            // Ожидаем подтверждения навигации по URL
            await Page.WaitForURLAsync($"**{expectedUrlPart}**");
            Log.Information("[Navigation] Успешно достигнут URL: {CurrentUrl}", Page.Url);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[Navigation] ОШИБКА. Не удалось загрузить '{Part}'. Текущий URL: {Actual}",
                expectedUrlPart, Page.Url);

            await TakeScreenshotAsync($"Error_Nav_{expectedUrlPart.Replace("/", "_")}");
            throw;
        }
    }*/

    // Передача метода записи шага в отчет из AllureService.cs
    /*protected async Task Do(string stepName, Func<Task> action)
    {
        // 1. Проверяем Log (Serilog)
        Log?.Information(stepName);

        // 2. Проверяем _test (ExtentReports) через ?.
        // Если _test равен null, выполнение просто пойдет дальше
        _test?.Info(stepName);

        // 3. Выполняем само действие через Allure
        await AllureService.Step(stepName, action);
    }

    // Передача метода записи шага получения результата в отчет из AllureService.cs 
    protected async Task<T> Do<T>(string stepName, Func<Task<T>> action)
    {
        Log.Information(stepName);
        _test?.Info(stepName);

        // Теперь AllureService.Step<T> возвращает значение, и ошибка CS0029 исчезнет
        return await AllureService.Step(stepName, action);
    }*/

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

    /// <summary>
    /// Создание скриншота всей страницы
    /// </summary>
    /*public async Task TakeScreenshotAsync(string name)
    {
        // 1. Создаем безопасное имя (убираем *, :, /, \ и т.д.)
        string safeActionName = name;
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            safeActionName = safeActionName.Replace(c, '_');
        }

        // 2. ИСПОЛЬЗУЕМ safeActionName вместо name для формирования пути!
        string fileName = $"{safeActionName}_{DateTime.Now:HHmmss}.png";
        string path = Path.Combine("screenshots", fileName);

        await Do($"[Screenshot] Фиксация экрана: {name}", async () =>
        {
            Directory.CreateDirectory("screenshots");

            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = path, // Здесь теперь будет путь без '*'
                FullPage = false
            });

            // В Allure можно передавать оригинальное name, там символы разрешены
            AllureService.AddAttachment(name, path);

            Log.Information("[Screenshot] Сохранен локально: {Path}", path);
        });
    }*/

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