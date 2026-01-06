using Xunit;
using Allure.Xunit;
// Эта директива указывает xUnit использовать AllureTestFramework для всех тестов в этой сборке.
[assembly: TestFramework("Allure.Xunit.AllureTestFramework", "Allure.Xunit")]