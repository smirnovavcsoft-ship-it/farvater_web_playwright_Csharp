using Xunit;
using FarvaterWeb.Setup;       

[CollectionDefinition("Playwright Test Collection")]
public class PlaywrightTestCollection : ICollectionFixture<PlaywrightFixture>
{
}