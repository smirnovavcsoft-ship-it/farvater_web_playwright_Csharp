using Xunit;

namespace FarvaterWeb.Base;

[CollectionDefinition("AllureCollection")]
public class AllureCollection : ICollectionFixture<AllureLauncher>
{
}