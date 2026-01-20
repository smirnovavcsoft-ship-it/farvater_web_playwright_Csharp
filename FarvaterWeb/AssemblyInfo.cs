using Xunit;

namespace FarvaterWeb.Base;

[CollectionDefinition("AllureCollection")]
public class AllureCollection : ICollectionFixture<AllureLauncher>
{
    // Этот класс не содержит кода. 
    // Он нужен только для того, чтобы xUnit знал: 
    // все тесты этой коллекции используют AllureLauncher.
}