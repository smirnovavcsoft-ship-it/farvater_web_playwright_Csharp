using AventStack.ExtentReports;
using AventStack.ExtentReports.Model;
using FarvaterWeb.Components;
using Microsoft.Playwright;
using Serilog;
using System.Text.RegularExpressions;

namespace FarvaterWeb.Base;

public abstract class BasePage : BaseComponent
{
   
    public TableComponent Table => new TableComponent(Page, Log, _test);

    public CancelComponent CancelAction => new CancelComponent(Page, Log, _test);

    
    protected BasePage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test, "Page")
    {
        
    }   
    
    public async Task GoToUrl(string url, string expectedUrlPart)
    {
        await Do($"[Navigation] Переход на URL: {url}", async () =>
        {
            await Page.GotoAsync(url);

            await Page.WaitForLoadStateAsync(LoadState.Load);

            await Assertions.Expect(Page).ToHaveURLAsync(new Regex($".*{expectedUrlPart}.*"), new() { Timeout = 10000 });

            await TakeScreenshotAsync($"Nav_{expectedUrlPart.Replace("/", "_")}");
        });
    }

    
    public async Task TakeScreenshotAsync(string name)
    {
        string safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
        safeName = safeName.Replace("*", "");       

        string fileName = $"step_{safeName}_{DateTime.Now:HH-mm-ss}.png";

        string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults", "Screenshots");
        string fullPath = Path.Combine(folderPath, fileName);

        Log.Information("[Debug] Сохраняем файл в: {FullPath}", fullPath);

        await Do($"[Screenshot] Фиксация экрана: {name}", async () =>
        {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = fullPath });

            AllureService.AddAttachment(name, fullPath);
        });
    }

    public async Task ScrollToBottom()
    {
        await Do("[Page] Прокрутка страницы вниз", async () =>
        {
            await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");

            await TakeScreenshotAsync("ScrollToBottom");
        });
    }

    public async Task AssertPageTitle(string expectedTitle)
    {
        await Do($"[Assertion] Проверка заголовка страницы: '{expectedTitle}'", async () =>
        {
            await Assertions.Expect(Page).ToHaveTitleAsync(expectedTitle);
        });
    }
}