using System.Diagnostics;

namespace FarvaterWeb.Base;

public class AllureLauncher : IDisposable
{
    public void Dispose()
    {
        bool enableAutoReport = false;
        // Этот код сработает ОДИН РАЗ после всех тестов в коллекции
        if (enableAutoReport && Environment.GetEnvironmentVariable("CI") == null)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c allure serve allure-results",
                    CreateNoWindow = true,
                    UseShellExecute = true
                });
            }
            catch { /* Логируем или игнорируем, если allure не установлен */ }
        }
    }
}