using Allure.Net.Commons;
using System;
using System.IO;
using System.Collections.Generic;

namespace FarvaterWeb.Base;

public static class AllureService
{
    public static bool IsEnabled { get; set; } = true;

    // Возвращаем StartTest, который требует BaseTest
    public static void StartTest(string uuid, string name, string fullName = "")
    {
        if (!IsEnabled) return;
        try
        {
            // Используем AllureApi для запуска (если версия позволяет) 
            // или стандартный Lifecycle для управления контейнером
            var testResult = new TestResult
            {
                uuid = uuid,
                name = name,
                fullName = string.IsNullOrEmpty(fullName) ? name : fullName,
                labels = new List<Label> { Label.Host(), Label.Thread() }
            };
            AllureLifecycle.Instance.StartTestCase(testResult);
        }
        catch { /* Контекст может быть уже запущен адаптером xUnit */ }
    }

    // Тот самый рабочий AddAttachment через Api
    public static void AddAttachment(string name, string path)
    {
        if (IsEnabled && File.Exists(path))
        {
            try
            {
                AllureApi.AddAttachment(name, "application/octet-stream", path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Allure] Ошибка вложения: {ex.Message}");
            }
        }
    }

    // Возвращаем Finish, который требует BaseTest
    public static void Finish(bool failed)
    {
        if (!IsEnabled) return;
        try
        {
            AllureLifecycle.Instance.UpdateTestCase(t =>
                t.status = failed ? Status.failed : Status.passed);
            AllureLifecycle.Instance.StopTestCase();
            AllureLifecycle.Instance.WriteTestCase();
        }
        catch { }
    }
}