using Xunit;
using Microsoft.Playwright;
using FarvaterWeb.Setup;

// Указываем xUnit, что этот класс тестов использует PlaywrightFixture
public class BasePageTests : IClassFixture<PlaywrightFixture>
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Конструктор получает экземпляр PlaywrightFixture через инъекцию зависимостей xUnit
    public BasePageTests(PlaywrightFixture fixture)
    {
        _page = fixture.Page;
        _baseUrl = fixture.BaseUrl;
    }

    [Fact]
    public async Task Test_Page_Title_Using_Fixture()
    {
        // Вместо создания браузера, мы используем _page
        await _page.GotoAsync(_baseUrl);

        var title = await _page.TitleAsync();

        Assert.Contains("Playwright", title);
    }
}