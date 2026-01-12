using Microsoft.Playwright;
using Serilog;
using AventStack.ExtentReports;

namespace FarvaterWeb.Base;

public abstract class BaseUI
{
    protected readonly IPage Page;
    protected readonly ILogger Log;
    protected readonly ExtentTest _test;
    protected readonly string _componentName;
    private static int _stepCounter = 0;

    protected BaseUI(IPage page, ILogger logger, ExtentTest test, string name = "UI")
    {
        Page = page;
        Log = logger;
        _test = test;
        _componentName = name;
    }

    // --- Главные методы для отчетов ---

    protected async Task Do(string stepName, Func<Task> action)
    {
        Log?.Information(stepName);
        _test?.Info(stepName);
        await AllureService.Step(stepName, action);
    }

    protected async Task<T> Do<T>(string stepName, Func<Task<T>> action)
    {
        Log?.Information(stepName);
        _test?.Info(stepName);
        return await AllureService.Step(stepName, action);
    }

    protected async Task AutoScreenshot(string actionName)
    {
        _stepCounter++;
        var fileName = $"step_{_stepCounter:D3}_{_componentName}_{actionName}.png";
        var path = Path.Combine(BaseTest.ScreenshotsPath, fileName);

        Directory.CreateDirectory(BaseTest.ScreenshotsPath);
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });

        var relativePath = $"../Screenshots/{fileName}";
        _test?.Info($"Шаг {_stepCounter}: {actionName}",
            MediaEntityBuilder.CreateScreenCaptureFromPath(relativePath).Build());

        AllureService.AddAttachment(actionName, path);
    }
}