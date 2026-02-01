using AventStack.ExtentReports;
using FarvaterWeb.Base;
using Microsoft.Playwright;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Components
{
    public class CalendarComponent : BaseComponent
    {
        private readonly string _label;
        private readonly string _pageName;

        public CalendarComponent(IPage page, ILogger logger, ExtentTest test, string label, string pageName = "Component")
            : base(page, logger, test)
        {
            _label = label;
            _pageName = pageName;
        }

        private ILocator InputContainer => Page.Locator("[data-signature='input-field-wrapper']")
            .Filter(new() { Has = Page.GetByText(_label, new() { Exact = true }) });

        private ILocator InputField => InputContainer.Locator("[data-signature='input-field-input']");

        private ILocator CalendarPopup => Page.Locator(".react-datepicker");

        public async Task SetDateAsync(DateTime date)
        {
            await Do($"[{_pageName}] Установка даты '{date:dd.MM.yyyy}' в поле '{_label}'", async () =>
            {
                await InputField.ClickAsync();

                await Assertions.Expect(CalendarPopup).ToBeVisibleAsync();

                await CalendarPopup.Locator("input[type='number']").FillAsync(date.Year.ToString());

                await CalendarPopup.Locator("select").SelectOptionAsync(new[] { (date.Month - 1).ToString() });

                var daySelector = $".react-datepicker__day--0{date.Day:D2}:not(.react-datepicker__day--outside-month)";
                await CalendarPopup.Locator(daySelector).First.ClickAsync();

                await Assertions.Expect(CalendarPopup).ToBeHiddenAsync();
            });
        }

        public async Task SetTodayAsync()
        {
            await Do($"Установка сегодняшней даты в поле '{_label}'", async () =>
            {
                await InputField.ClickAsync();

                var todayButton = CalendarPopup.Locator(".react-datepicker__today-button");

                await Assertions.Expect(todayButton).ToBeVisibleAsync();

                await todayButton.ClickAsync();

                await Assertions.Expect(CalendarPopup).ToBeHiddenAsync();

                string todayStr = DateTime.Now.ToString("dd.MM.yyyy");
                await Assertions.Expect(InputField).ToHaveValueAsync(todayStr);
            });
        }
    }
}
