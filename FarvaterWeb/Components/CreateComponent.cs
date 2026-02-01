using AventStack.ExtentReports;
using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;

public class CreateComponent : BaseComponent
{
    public CreateComponent(IPage page, ILogger logger, ExtentTest test)
        : base(page, logger, test, "Компонент создания") { }

    public async Task CreateAndVerify(string buttonName, string verifyText)
    {
        await Do($"Создание объекта через кнопку '{buttonName}' и проверка '{verifyText}'", async () =>
        {
            await DoClickByText(buttonName);

            var row = Page.Locator("tr").Filter(new() { HasText = verifyText });

            await Assertions.Expect(row).ToBeVisibleAsync(new() { Timeout = 10000 });

            await TakeScreenshotAsync($"Created_{verifyText.Replace(" ", "_")}");
        });
    }
}