using Microsoft.Playwright;

public class SmartLocator
{
    public ILocator Locator { get; }
    public string Name { get; }
    public string Type { get; }
    public IPage Page { get; }

    public SmartLocator(ILocator locator, string name, string type, IPage page)
    {
        Locator = locator;
        Name = name;
        Type = type;
        Page = page;
    }
}