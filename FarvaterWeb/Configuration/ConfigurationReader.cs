using Microsoft.Extensions.Configuration;
using System.IO;

namespace FarvaterWeb.Configuration
{
    // Класс для чтения конфигурации из appsettings.json
    public static class ConfigurationReader
    {
        private static IConfigurationRoot _configuration;

        // Статический конструктор для инициализации конфигурации один раз
        static ConfigurationReader()
        {
            // Установка базового пути к каталогу, где находится appsettings.json
            _configuration = new ConfigurationBuilder()
                // Устанавливаем текущий каталог как базу
                .SetBasePath(Directory.GetCurrentDirectory())
                // Добавляем appsettings.json; optional: false означает, что файл обязателен
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        /// <summary>
        /// Получает базовый URL из секции PlaywrightSettings
        /// </summary>
        public static string BaseUrl => _configuration["PlaywrightSettings:BaseUrl"]!;

        /// <summary>
        /// Получает тип браузера ("Chromium", "Firefox" или "WebKit")
        /// </summary>
        public static string BrowserType => _configuration["PlaywrightSettings:BrowserType"]!;

        /// <summary>
        /// Получает настройку Headless (true/false)
        /// </summary>
        public static bool Headless => bool.Parse(_configuration["PlaywrightSettings:Headless"]!);

        // Здесь можно добавить другие настройки, например, тайм-ауты.
    }
}