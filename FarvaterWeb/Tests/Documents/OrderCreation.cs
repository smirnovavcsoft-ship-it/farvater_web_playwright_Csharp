using FarvaterWeb.ApiServices;
using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{
    /*public class OrderCreationTest : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);

        private OrdersPage Orders => new OrdersPage(Page, Log, _test);

        private UserApiService UserApi => new UserApiService(ApiRequest);

        public OrderCreationTest(ITestOutputHelper output) : base(output) { }

        public async Task ShouldCreateOrder()
        {
            string postfix = DataPostfixExtensions.GetUniquePostfix();
            string lastName = "Тестерович";
            string firstName = "Андрей";
            string login = $"lastName{postfix}";

            string? userHandle = null;

            try

            {
                userHandle = await UserApi.PrepareUserAsync(lastName, firstName, login);


                Log.Information("--- Запуск сценария: Создание записки---");
                await LoginAsAdmin();
                await SideMenu.OpenSection("Записки", "notes");

                // Нажатие кнопки "Создание документа"



                // Заполнение данных приказа

                var orderDetails = new OrderDetails
                    (
                    "Распоряжение",
                    "Наименование приказа",
                    "Содержание приказа",
                    lastName,
                    firstName
                    );

                // Проверка на кнопки "Отмена"

                // Нажатие кнопки "Создание документа"

                // Заполнение данных приказа

                // Нажатие кнопки "Создать"

                // Удаление созданного приказа

                // Удаление созданного пользователя пользователя
            }
    }*/
}
