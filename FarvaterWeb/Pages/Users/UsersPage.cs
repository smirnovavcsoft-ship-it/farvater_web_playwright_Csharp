using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Extensions;
using FarvaterWeb.Tests.Counterparty;
using FarvaterWeb.Tests.Users;
using HarmonyLib;
using Microsoft.Playwright;
using Serilog;
using System.Xml.Linq;

namespace FarvaterWeb.Pages.Users
{
    public class UsersPage : BasePage
    {
        // Находим контейнер вкладок по уникальному классу
        private ILocator TabsContainer => Page.Locator("div[class*='_switchContainer_']");

        // Конкретная вкладка внутри этого контейнера
        private ILocator UsersTab => TabsContainer.GetByText("Пользователи", new() { Exact = true });

        private const string DepartmentField = "Подразделение";
        public UsersPage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test) { }

        public async Task ClickTab(string tabName)
        {
            Log.Information("[UsersPage] Переход на вкладку: {TabName}", tabName);

            // Используем локатор по тексту
            await Page.GetByText(tabName, new() { Exact = true }).ClickAsync();

            // Опционально: подождать, пока вкладка станет активной 
            // (в вашем HTML активная вкладка помечается наличием внутри div класса _switchOutline_)
        }

        public async Task ClickUsersTab()
        {        
           
            await Do("Переход на вкладку: Пользователи", async () =>
            {
                // Ждем, чтобы вкладка стала видимой (на случай долгой загрузки)
                //await UsersTab.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                // Кликаем именно по вкладке внутри контейнера
                await UsersTab.ClickAsync();
            });
        }

        public async Task ClickCreatePositionButton()
        {
            //await DoClickByText("Создать должность");
            //Новый метод с расширением
            await Button("Создать должность").SafeClickAsync();
        }

        public async Task ClickCreateDepartmentButton()
        {
            //await DoClickByText("Создать подразделение");
            //Новый метод с расширением
            await Button("Создать подразделение").SafeClickAsync();
        }

        
        public async Task ClickCreateGroupButton()
        {
            //await DoClickByText("Создать группу");
            //Новый метод с расширением
            await Button("Создать группу").SafeClickAsync();
        }

        public async Task ClickAddUser()
        {
            //await DoClickByText("Добавить пользователя");
            //Новый метод с расширением
            await Button("Добавить пользователя").SafeClickAsync();
        }

        /*public async Task DeletePosition(string name)
        {
            // Указываем специфичный селектор корзины для этой страницы
            // Точка в начале означает поиск по классу
            //const string deleteIcon = ".menuItemDelete";

            // Вызываем метод компонента
            await Table.DeleteRow(name);

            // Подтверждаем удаление в модальном окне (это уже логика страницы или ModalComponent)
            await Page.GetByRole(AriaRole.Button, new() { Name = "Да" }).ClickAsync();


        }*/

        public async Task FillPositionName(string name)
        {
            await DoFillByLabel("Наименование", name);
        }

        public async Task FillDepartmentDetails(DepartmentDetails details)
        {
            await DoFillByLabel("Наименование", details.Name);
            await DoFillByLabel("Код", details.Code);
        }

        public async Task FillUserDetails(UserDetails details)
        {
            Log.Information("[UsersPage] Заполнение формы по заголовкам");
            await DoFillByLabel("Фамилия", details.Lastname);
            await DoFillByLabel("Имя", details.Name);
            await DoFillByLabel("Отчество", details.Middlename);
            await DoFillByLabel("Таб. \u2116", details.IDnumber);
            await DoFillByLabel("Логин", details.UserLogin);
            await DoFillByLabel("Телефон", details.Phone);
            await DoFillByLabel("E-mail", details.Email);
        }

        public async Task FillGroupName(string name)
        {
            await DoFillByLabel("Наименование", name);
        }

        public async Task SetPermissions(PermissionDetails permissions)
        {
            await Do("Установка прав доступа для роли", async () =>
            {
                // Вызываем твой универсальный метод для каждого права
                await SetCheckboxByText("Администратор", permissions.IsAdmin);
                await SetCheckboxByText("ГИП", permissions.IsGip);
                await SetCheckboxByText("Архив", permissions.IsArchive);
                await SetCheckboxByText("Работа с договорами", permissions.IsContracts);
                await SetCheckboxByText("Работа с ОРД", permissions.IsOrd);
            });
        }



        public async Task CancelAndVerify(string PositionName)
        {
            
            await CancelAction.CancelAndVerify(PositionName);
        }

        public async Task ClickAddButton()
        {
            //await DoClickByText("Добавить");
            //Новый метод с расширением
            await Button("Добавить").SafeClickAsync();
        }

        /*public async Task ClickAddButton()
        {
            await DoClickByText("Добавить");
        }*/

        public async Task VerifyPositionCreated(string positionName)
        {
            // Метод AssertTextExists уже доступен, так как страница наследует BaseComponent
            await AssertTextExists(positionName);
        }

        

        public async Task DeletePosition(string positionName)
        {
            string buttonText = "Удалить";
            // 1. Нажимаем на иконку удаления в таблице
            await Table.DeleteRow(positionName, buttonText);

            // 2. Ждем появления модального окна и кликаем по кнопке подтверждения
            // Используем селектор кнопки "Удалить" в модальном окне
            // Обычно у нее есть уникальный класс или текст
            await Do($"Подтверждение удаления должности '{positionName}'", async () =>
            {
                var confirmButton = Page.GetByRole(AriaRole.Button, new() { Name = "Удалить", Exact = true });

                // Ждем, чтобы кнопка стала видимой (на случай анимации появления модалки)
                //await confirmButton.WaitForAsync(new() { State = ElementState.Visible, Timeout = 5000 });

                //await confirmButton.ClickAsync();

                // 3. Ждем, пока модалка исчезнет (хорошая практика)
                //await Assertions.Expect(confirmButton).ToBeHiddenAsync();
            });
        }

        public async Task DeleteDepartment(string departmentName, string departmentCode)
        {
            string buttonText = "Удалить";
            await Table.DeleteRow(departmentName, buttonText);
        }

        public async Task DeleteGroup(string groupName)
        {
            string buttonText = "Удалить";
            await Table.DeleteRow(groupName, buttonText);
        }

        public async Task SelectDepartmentByNumber(int position)
        {
            // Вызываем наш метод из BaseComponent, который мы написали ранее
            await SelectDropdownItemByNumber(DepartmentField, position);
        }



    }
}
