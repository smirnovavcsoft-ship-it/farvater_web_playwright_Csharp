using AventStack.ExtentReports;
using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;

public class CancelComponent : BaseComponent
{
    // 1. УДАЛИЛИ private readonly IPage _page;

    public CancelComponent(IPage page, ILogger logger, ExtentTest test)
        : base(page, logger, test) { }

    public async Task CancelAndVerify(string unexpectedText)
    {
        // Используем Do, чтобы это отразилось в отчетах
        await Do($"Проверка отмены для: {unexpectedText}", async () =>
        {
            // 2. Используем Page (с большой буквы) из BaseComponent
            var cancelButton = Page.GetByRole(AriaRole.Button, new() { Name = "Отмена" });

            await cancelButton.ClickAsync();

            await Assertions.Expect(cancelButton).ToBeHiddenAsync(new() { Timeout = 5000 });

            var row = Page.Locator("tr").Filter(new() { HasText = unexpectedText });
            await Assertions.Expect(row).ToBeHiddenAsync(new() { Timeout = 3000 });

            await AutoScreenshot("CancelVerify");
        });
    }
}