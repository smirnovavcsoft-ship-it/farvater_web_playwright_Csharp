using Serilog;
using FarvaterWeb.Services; // Для доступа к AllureService

namespace FarvaterWeb.Base
{
    public static class HelperForReports
    {
        public static async Task Do(string stepName, Func<Task> action)
        {
            // 1. Логируем в консоль (Serilog)
            Log.Information($"[STEP] {stepName}");

            // 2. Отправляем в Allure
            // Это магическая связь: Allure привяжет шаг к текущему тесту
            await AllureService.Step(stepName, action);
        }
    }
}