// PlaywrightTestCollection.cs (Новый файл, если его нет)
using Xunit;
using FarvaterWeb.Setup; // Укажите правильный namespace, где находится PlaywrightFixture

// Определяем коллекцию тестов и связываем ее с PlaywrightFixture
[CollectionDefinition("Playwright Test Collection")]
public class PlaywrightTestCollection : ICollectionFixture<PlaywrightFixture>
{
    // Класс пуст, его назначение - только связь фикстуры и коллекции.
}