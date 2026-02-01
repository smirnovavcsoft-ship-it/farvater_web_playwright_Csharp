using AventStack.ExtentReports;
using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarvaterWeb.Extensions;

namespace FarvaterWeb.Components
{
    public class SideMenuComponent : BaseComponent
    {
        private readonly string _pageName;
        private ILocator MenuRoot => Page.Locator("div._sideMenu_viwlf_12");
        
        public SideMenuComponent(IPage page, ILogger logger, ExtentTest test, string pageName = "Component")
        : base(page, logger, test, "Боковое меню")
        {
            _pageName = pageName;
            
        }

       

        public SmartLocator MenuItem(string name)
        {
            var locator = MenuRoot.Locator("a")
                              .Filter(new() { HasText = name, Visible = true });

            return new SmartLocator(locator, name, "Пункт меню", "Link", Page);
        }

        public async Task ClickItem(string name)
        {
            await Page.Do($"[{_pageName}] Клик по пункту меню '{name}'", async () =>
            {
                await MenuItem(name).Locator.First.ClickAsync();
            });
        }
    }
}
