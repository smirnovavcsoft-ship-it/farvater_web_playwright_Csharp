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
    public class DateComponent : BaseComponent
    {
        private readonly string _label;
        private readonly string _pageName;

        public DateComponent(IPage page, ILogger logger, ExtentTest test, string label, string pageName = "Component")
            : base(page, logger, test)
        {
            _label = label;
            _pageName = pageName;
        }

        // 1. Локатор контейнера инпута (ищем по тексту заголовка внутри сигнатуры)
        /*private ILocator InputContainer => Page.Locator("[data-signature='input-field-wrapper']")
            .Filter(new() { HasText = _label });*/

        private ILocator InputContainer => Page.Locator("[data-signature='input-field-wrapper']")
            .Filter(new() { Has = Page.GetByText(_label, new() { Exact = true }) });

        private ILocator InputField => InputContainer.Locator("[data-signature='input-field-input']");

        // 2. Локатор самого всплывающего календаря (он обычно вне контейнера инпута)
        private ILocator CalendarPopup => Page.Locator(".react-datepicker");

        public async Task SetDateAsync(DateTime date)
        {
            await Do($"[{_pageName}] Установка даты '{date:dd.MM.yyyy}' в поле '{_label}'", async () =>
            {
                // Открываем календарь кликом
                await InputField.ClickAsync();

                // Ждем появления окна
                await Assertions.Expect(CalendarPopup).ToBeVisibleAsync();

                // Вместо того чтобы мучаться с кнопками «Вперед/Назад», 
                // используем то, что разработчики дали нам <select> для месяца и <input> для года

                // Выбираем год (в твоем HTML это input type="number")
                await CalendarPopup.Locator("input[type='number']").FillAsync(date.Year.ToString());

                // Выбираем месяц (в твоем HTML это <select>)
                // Значение в селекте начинается с 0 (Январь = 0)
                await CalendarPopup.Locator("select").SelectOptionAsync(new[] { (date.Month - 1).ToString() });

                // Выбираем день. В твоем HTML у дней есть отличный aria-label: "Choose вторник, 27 января 2026 г."
                // Мы можем искать по числу, игнорируя соседние месяцы (outside-month)
                var daySelector = $".react-datepicker__day--0{date.Day:D2}:not(.react-datepicker__day--outside-month)";
                await CalendarPopup.Locator(daySelector).First.ClickAsync();

                // Проверяем, что календарь закрылся
                await Assertions.Expect(CalendarPopup).ToBeHiddenAsync();
            });
        }

        public async Task SetTodayAsync()
        {
            await Do($"Установка сегодняшней даты в поле '{_label}'", async () =>
            {
                // 1. Открываем календарь
                await InputField.ClickAsync();

                // 2. Ждем появления кнопки "Сегодня"
                // В react-datepicker она обычно имеет класс .react-datepicker__today-button
                var todayButton = CalendarPopup.Locator(".react-datepicker__today-button");

                await Assertions.Expect(todayButton).ToBeVisibleAsync();

                // 3. Кликаем по ней
                await todayButton.ClickAsync();

                // 4. Проверяем, что календарь закрылся и дата подставилась
                await Assertions.Expect(CalendarPopup).ToBeHiddenAsync();

                string todayStr = DateTime.Now.ToString("dd.MM.yyyy");
                await Assertions.Expect(InputField).ToHaveValueAsync(todayStr);
            });
        }
    }
}
