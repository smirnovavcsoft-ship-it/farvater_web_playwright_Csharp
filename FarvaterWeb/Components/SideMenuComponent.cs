using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FarvaterWeb.Components
{
    public class SideMenuComponent : BaseComponent
    {
        private readonly string _pageName;
        private ILocator MenuRoot => Page.Locator("div._sideMenu_viwlf_12");
        
        // Конструктор передает в базу селектор именно бокового меню
        public SideMenuComponent(IPage page, ILogger logger, ExtentTest test, string pageName = "Component")
        : base(page, logger, test, "Боковое меню")
        {
            _pageName = pageName;
            
        }

       

        // Вспомогательный метод для получения SmartLocator-а конкретной ссылки
        // Мы ищем только те ссылки, у которых есть data-signature='page-link-wrapper'
        public SmartLocator MenuItem(string name)
        {
            // Ищем любой элемент 'a' внутри бокового меню, который содержит указанный текст
            // Фильтр Visible гарантирует, что мы не пытаемся кликнуть на скрытый элемент
            var locator = MenuRoot.Locator("a")
                              .Filter(new() { HasText = name, Visible = true });

            return new SmartLocator(locator, name, "Пункт меню", "Link", Page);
        }

        // Метод действия: клик по пункту
        public async Task ClickItem(string name)
        {
            await Page.Do($"[{_pageName}] Клик по пункту меню '{name}'", async () =>
            {
                // Берем наш SmartLocator и вызываем клик
                // Используем First, чтобы Playwright не ругался на строгость внутри меню
                await MenuItem(name).Locator.First.ClickAsync();
                string newUrl = Page.Url;
                Log.Information($"Переход на страницу: {newUrl}");

            });
        }
    }
}
