using Microsoft.Playwright;

public class SmartLocator
{
    public ILocator Locator { get; }
    public string Name { get; }

    // Меняем private поля на public свойства, чтобы расширения их видели
    public IPage Page { get; }
    public string ComponentName { get; }
    public string Type { get; } // Это то, что раньше было _elementType

    public SmartLocator(ILocator locator, string name, string elementType, string componentName, IPage page)
    {
        Locator = locator;
        Name = name;
        Type = elementType;
        ComponentName = componentName;
        Page = page;
    }

    public SmartLocator PlusButton => new SmartLocator(
        Locator.Locator("button[data-signature='button-wrapper']"),
        $"Кнопка '+' для '{Name}'",
        "кнопка добавления",
        ComponentName,
        Page);
}
