using Microsoft.Playwright;

public class SmartLocator
{
    public ILocator Locator { get; }
    public string Name { get; }
    public string Type { get; }
    public string ComponentName { get; } // Сюда придет значение из _componentName
    public IPage Page { get; }

    public SmartLocator(ILocator locator, string name, string type, string componentName, IPage page)
    {
        Locator = locator;
        Name = name;
        Type = type;
        ComponentName = componentName;
        Page = page;
    }
}