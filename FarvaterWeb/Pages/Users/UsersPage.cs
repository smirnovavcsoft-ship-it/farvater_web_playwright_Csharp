using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Extensions;
using FarvaterWeb.Tests.Counterparty;
using FarvaterWeb.Tests.Users;
using HarmonyLib;
using Microsoft.Playwright;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace FarvaterWeb.Pages.Users
{
    public class UsersPage : BasePage
    {
        private ILocator TabsContainer => Page.Locator("div[class*='_switchContainer_']");

        private ILocator UsersTab => TabsContainer.GetByText("Пользователи", new() { Exact = true });

        private SmartLocator DepartmentDropdown => Dropdown.WithLabel("Подразделение");
        private SmartLocator PositionDropdown => Dropdown.WithLabel("Должность");

        private SmartLocator DepartmentPlusButton => Dropdown.WithLabel("Подразделение").PlusButton;

        private SmartLocator PositionPlusButton => Dropdown.WithLabel("Должность").PlusButton;

        private SmartLocator ReplacementEmployeeDropdown => Dropdown
            .WithLocator(Page.Locator("//div[contains(@class, 'employee-select')]")
            .Last, "Выбор замещающего сотрудника");

        private SmartLocator ResponsiblePersonsSelectionDropdown => Dropdown.WithLabel("Ответственные");

        

        private SmartLocator PositionInput => Input.WithText("Введите наименование");

        private SmartLocator DepartmentNameInput => Input.WithLocator(
            Page.Locator("input[name='description']").Locator("visible=true").First,
            "Наименование"
                );


        private SmartLocator DepartmentCodeInput => Input.WithText("Введите код");

        
        public UsersPage(IPage page, ILogger logger, ExtentTest test) : base(page, logger, test) { }

        public async Task ClickTab(string tabName)
        {
            Log.Information("[UsersPage] Переход на вкладку: {TabName}", tabName);

            await Page.GetByText(tabName, new() { Exact = true }).ClickAsync();

        }

        public async Task ClickUsersTab()
        {        
           
            await Do("Переход на вкладку: Пользователи", async () =>
            {
                await UsersTab.ClickAsync();
            });
        }

        public async Task ClickCreatePositionButton()
        {
            await ButtonWithText("Создать должность").SafeClickAsync();
        }

        public async Task ClickCreateDepartmentButton()
        {
            await ButtonWithText("Создать подразделение").SafeClickAsync();
        }


        
        public async Task ClickCreateGroupButton()
        {
            await ButtonWithText("Создать группу").SafeClickAsync();
        }

        public async Task ClickAddUser()
        {
            await ButtonWithText("Добавить пользователя").SafeClickAsync();
        }

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
            await DoFillByLabel("Фамилия", details.Lastname);
            await DoFillByLabel("Имя", details.Name);
            await DoFillByLabel("Отчество", details.Middlename);
            await DoFillByLabel("Таб. \u2116", details.IDnumber);
            await DoFillByLabel("Логин", details.UserLogin);
            await DoFillByLabel("Телефон", details.Phone);
            await DoFillByLabel("E-mail", details.Email);            

        }

        public async Task SelectFirstDepartment()
        {
            await DepartmentDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task SelectFirstPosition()
        {
            await PositionDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task SelectFirstResponsiblePerson()
        {
            await ResponsiblePersonsSelectionDropdown.SelectByIndexAndVerifyAsync(0);
        }


        public async Task CreateDepartmentInUserCard(DepartmentDetails details)
        {
            await DepartmentPlusButton.SafeClickAsync();
            await DepartmentNameInput.ClearAndFillAsync(details.Name);
            await Assertions.Expect(DepartmentNameInput.Locator).ToHaveCountAsync(1);
            await DepartmentCodeInput.ClearAndFillAsync(details.Code);
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

        public async Task SetPermissions(PermissionDetails permissions)
        {
            await Do("Установка прав доступа для роли", async () =>
            {
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
            await ButtonWithText("Добавить").SafeClickAsync();
        }

        public async Task ClickCreateButton()
        {
            await ButtonWithText("Создать").SafeClickAsync();
        }

        public async Task VerifyPositionCreated(string positionName)
        {
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
            await Table.DeleteRow(positionName, buttonText);

            await Do($"Подтверждение удаления должности '{positionName}'", async () =>
            {
                var confirmButton = Page.GetByRole(AriaRole.Button, new() { Name = "Удалить", Exact = true });

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
            await ReplacementEmployeeDropdown.SelectByIndexAndVerifyAsync(0);
        }

        public async Task ClickFireButton2()
        {
            await ButtonWithText("Уволить").SafeClickAsync();
        }
           

    }
}
