using AventStack.ExtentReports;
using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;

public class CreateComponent : BaseComponent
{
    public CreateComponent(IPage page, ILogger logger, ExtentTest test)
        : base(page, logger, test, "Компонент создания") { }

    /// <param name="buttonName">Текст на кнопке (Добавить, Создать, Ок и т.д.)</param>
    /// <param name="verifyText">Текст, который должен появиться в таблице после создания</param>
    public async Task CreateAndVerify(string buttonName, string verifyText)
    {
        await Do($"Создание объекта через кнопку '{buttonName}' и проверка '{verifyText}'", async () =>
        {
            // Используем наш универсальный клик по тексту кнопки
            await DoClickByText(buttonName);

            // Ждем появления строки с текстом в таблице
            var row = Page.Locator("tr").Filter(new() { HasText = verifyText });

            // Ставим таймаут чуть больше, так как серверу нужно время на запись
            await Assertions.Expect(row).ToBeVisibleAsync(new() { Timeout = 10000 });

            await TakeScreenshotAsync($"Created_{verifyText.Replace(" ", "_")}");
        });
    }
}