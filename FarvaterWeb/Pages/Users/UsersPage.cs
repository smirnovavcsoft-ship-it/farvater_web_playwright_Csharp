using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using FarvaterWeb.Tests.Counterparty;
using FarvaterWeb.Tests.Users;
using HarmonyLib;
using Microsoft.Playwright;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using FarvaterWeb.TestData;
using FarvaterWeb.Generators;

namespace FarvaterWeb.Pages.Users
{
    public class UsersPage : BasePage
    {
        // Находим контейнер вкладок по уникальному классу
        private ILocator TabsContainer => Page.Locator("div[class*='_switchContainer_']");

        // Конкретная вкладка внутри этого контейнера
        private ILocator UsersTab => TabsContainer.GetByText("Пользователи", new() { Exact = true });

        //private const string DepartmentField = "Подразделение";

        private SmartLocator DepartmentDropdown => Dropdown.WithLabel("Подразделение");
        private SmartLocator PositionDropdown => Dropdown.WithLabel("Должность");

        private SmartLocator DepartmentPlusButton => Dropdown.WithLabel("Подразделение").PlusButton;

        private SmartLocator PositionPlusButton => Dropdown.WithLabel("Должность").PlusButton;

        private SmartLocator ReplacementEmployeeDropdown => Dropdown
            .WithLocator(Page.Locator("//div[contains(@class, 'employee-select')]")
            .Last, "Выбор замещающего сотрудника");

        private SmartLocator ResponsiblePersonsSelectionDropdown => Dropdown.WithLabel("Ответственные");

        

        private SmartLocator PositionInput => Input.WithText("Введите наименование");

        // Группы

        private SmartLocator GroupNameInput => Input.WithLabel("Название");

        private SmartLocator ResponsibleDropdown => Dropdown.WithInputAndLabel("Ответственные");

        //private SmartLocator DepartmentNameInput => Input.WithText("Введите наименование");
        // private SmartLocator DepartmentNameInput => Input.WithLocator(Page.Locator("input[placeholder*='Введите наименование']")
        // .First, "Наименование");
        /*private SmartLocator DepartmentNameInput => Input.WithLocator(
            Page.Locator("[data-signature='input-field-wrapper']")
            .Filter(new() { HasText = "Наименование" })
            .Locator("input")
            .First,
            "Наименование"
                );*/
        /*private SmartLocator DepartmentNameInput => Input.WithLocator(
        Page.Locator("input[name='description']").First,
            "Наименование"
            );*/

        private SmartLocator DepartmentNameInput => Input.WithLocator(
            Page.Locator("input[name='description']").Locator("visible=true").First,
            "Наименование"
                );


        private SmartLocator DepartmentCodeInput => Input.WithText("Введите код");

        //private SmartLocator DepartmentField => Dropdown.WithLabel("Подразделение");
        //private SmartLocator PositionField => Dropdown.WithLabel("Должность");

        
        //private SmartLocator ReplacementUserField => Dropdown.WithSelector("div[data-testid='user-selector']", "Замещающий");
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
            await ButtonWithText("Создать должность").SafeClickAsync();
        }

        public async Task ClickCreateDepartmentButton()
        {
            //await DoClickByText("Создать подразделение");
            //Новый метод с расширением
            await ButtonWithText("Создать подразделение").SafeClickAsync();
        }


        
        public async Task ClickCreateGroupButton()
        {
            //await DoClickByText("Создать группу");
            //Новый метод с расширением
            await ButtonWithText("Создать группу").SafeClickAsync();
        }

        public async Task ClickAddUser()
        {
            //await DoClickByText("Добавить пользователя");
            //Новый метод с расширением
            await ButtonWithText("Добавить пользователя").SafeClickAsync();
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
            await DoFillByLabel1("Наименование*", details.Name);
            await DoFillByLabel("Код", details.Code);
        }

        public async Task FillUserDetails(UserDetails details)
        {
            Log.Information("[UsersPage] Заполнение формы по заголовкам");
            await DoFillByLabel("Фамилия", details.LastName);
            await DoFillByLabel("Имя", details.FirstName);
           // await DoFillByLabel("Отчество", details.Middlename);
            await DoFillByLabel("Таб. \u2116", details.IdNumber);
            await DoFillByLabel("Логин", details.Login);
            await DoFillByLabel("Телефон", details.Phone);
            await DoFillByLabel("E-mail", details.Email);            

        }

        public async Task SelectDepartment(departmentName)
        {
            await DepartmentDropdown.SelectByTextAndVerifyAsync(departmentName);
        }

        public async Task SelectPosition()
        {
            await PositionDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task SelectFirstResponsiblePerson()
        {
            await ResponsiblePersonsSelectionDropdown.SelectByIndexAndVerifyAsync(0);
        }


        public async Task CreateDepartmentInUserCard(DepartmentDetails details)
        {
            //await DepartmentField.CreateButton.SafeClickAsync();

            await DepartmentPlusButton.SafeClickAsync();
            await DepartmentNameInput.ClearAndFillAsync(details.Name);
            await Assertions.Expect(DepartmentNameInput.Locator).ToHaveCountAsync(1);
            await DepartmentCodeInput.ClearAndFillAsync(details.Code);
            //await DepartmentNameInput.AssertTextAsync(details.Name);
            //await DepartmentCodeInput.AssertTextAsync(details.Code);

        }

        public async Task CreatePositionInUserCard(string positionName)
        {
            await PositionPlusButton.SafeClickAsync();
            await PositionInput.ClearAndFillAsync(positionName);
        }



        public async Task FillGroupName(string name)
        {
            await DoFillByLabel("Название", name);
        }

        public async Task FillGroupDetails(GroupDetails details, UserModel newUser)
        {           
          
                await GroupNameInput.ClearAndFillAsync(details.GroupName);
                await ResponsibleDropdown.SelectUserAndVerifyAsync(newUser.LastName, newUser.FirstName);
                await SetCheckboxByText("Администратор", details.IsAdmin);
                await SetCheckboxByText("ГИП", details.IsGip);
                await SetCheckboxByText("Архив", details.IsArchive);
                await SetCheckboxByText("Работа с договорами", details.IsContracts);
                await SetCheckboxByText("Работа с ОРД", details.IsOrd);            
        }



        public async Task CancelAndVerify(string PositionName)
        {
            
            await CancelAction.CancelAndVerify(PositionName);
        }

        public async Task ClickAddButton()
        {
            //await DoClickByText("Добавить");
            //Новый метод с расширением
            await ButtonWithText("Добавить").SafeClickAsync();
        }

        /*public async Task ClickAddButton()
        {
            await DoClickByText("Добавить");
        }*/

        public async Task ClickCreateButton()
        {
            await ButtonWithText("Создать").SafeClickAsync();
        }

        public async Task VerifyPositionCreated(string positionName)
        {
            // Метод AssertTextExists уже доступен, так как страница наследует BaseComponent
            await AssertTextExists(positionName);
        }

        public async Task VerifyUserCreated(string userName)
        {

            await Page.ReloadAsync();
            await AssertTextExists(userName);
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

        public async Task DeleteDepartment(string departmentName)
        {
            string buttonText = "Удалить";
            await Table.DeleteRow(departmentName, buttonText);
        }

        public async Task DeleteGroup(string groupName)
        {
            string buttonText = "Удалить";
            await Table.DeleteRow(groupName, buttonText);
        }



        /*public async Task SelectDepartmentByNumber( int position)
        {
                       // Вызываем наш метод из BaseComponent, который мы написали ранее
            await SelectDropdownItemByNumber("Подразделение", position);
        }*/

        /*public async Task SelectDepartmentByNumber(int number)
        {
            // Теперь нам не нужно вызывать Do здесь, так как он уже внутри Safe-метода
            await Dropdown("Подразделение").SelectByIndexAndVerifyAsync(number - 1);
        }*/

        public async Task IsABoss()
        {
            await Checkbox("Является руководителем").SetAsync(true);
        }

        public async Task HaveARightToSign()
        {
            await Checkbox("Имеет право подписи").SetAsync(true);
        }

        public async Task OpenUserCard(string email)
        {
            await Table.Row(email).SafeClickAsync();
        }

        public async Task ClickFireButton1()
        {
            await ButtonWithText("Уволить").SafeClickAsync();
        }

        public async Task SelectReplacementEmployee()
        {
            //var replacementPath = Page.Locator("//div[contains(@class, 'employee-select')]").First;
            await ReplacementEmployeeDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task ClickFireButton2()
        {
            await ButtonWithText("Уволить").SafeClickAsync();
        }
           

    }
}
