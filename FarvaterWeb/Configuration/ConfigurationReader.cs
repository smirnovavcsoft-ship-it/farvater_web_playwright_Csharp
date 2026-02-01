using Microsoft.Extensions.Configuration;
using System.IO;

namespace FarvaterWeb.Configuration
{
    public static class ConfigurationReader
    {
        private static IConfigurationRoot _configuration;

        static ConfigurationReader()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string BaseUrl => _configuration["PlaywrightSettings:BaseUrl"]!;

        public static string BrowserType => _configuration["PlaywrightSettings:BrowserType"]!;

        public static bool Headless => bool.Parse(_configuration["PlaywrightSettings:Headless"]!);

        public static string Username => _configuration["UserCredentials:Username"]!;

        public static string Password => _configuration["UserCredentials:Password"]!;
    }
}