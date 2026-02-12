using AventStack.ExtentReports;
using FarvaterWeb.Base;
using FarvaterWeb.Components;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using Serilog;

namespace FarvaterWeb.Pages.Documents
{
    public class NotesPage : BasePage
    {
        private TableComponent Table => new TableComponent(Page, Log, _test);
        private SmartLocator CreateDocumentButton => ButtonWithText("Создать документ");
        private SmartLocator DocumentTypeDropdown => Dropdown.WithLabel("Тип документа");

        private SmartLocator TopicInput => Input.WithLabel("Тема");

        private SmartLocator ContentInput => Input.DescriptionField("Содержание *");

        private SmartLocator AdresseesDropdown => Dropdown.WithInputAndLabel("Адресаты");

        private SmartLocator CancelButton => ButtonWithText("Отмена");

        private SmartLocator CreateButton => ButtonWithText("Создать");

        public NotesPage(IPage page, ILogger logger, ExtentTest _test) : base(page, logger, _test) { }

        public async Task ClickCreateDocumentButton ()
        {
            await CreateDocumentButton.SafeClickAsync();
        }

        public async Task FillNoteDetails (NoteDetails details)
        {
            string text = "";
            await DocumentTypeDropdown.SelectByTextAndVerifyAsync(details.DocumentType);
            await TopicInput.ClearAndFillAsync(details.Topic);
            await ContentInput.ClearAndFillAsync(details.Content);
            await SelectAdressee(text);
        }

        public async Task CancelAndVerify(string topic)
        {
            await CancelAction.CancelAndVerify(topic);
        }

        public async Task ClickCreateButton()
        {
            await CreateButton.SafeClickAsync();
        }

        public async Task DeleteCreatedNote(string topic)
        {
            string buttonText = "Удалить";
            await Table.DeleteRow(topic, buttonText);
        }

        public async Task SelectAdressee( string text)
        {
            string stepName = $"[{smart.ComponentName}] Выбор пункта '{text}' в {smart.Type} '{smart.Name}'";

            await smart.Page.Do(stepName, async () =>
            {
                // 1. Открываем список
                await smart.Locator.ClickAsync(new() { Force = true });

                // 2. Находим контейнер с опциями (только видимый)
                var optionsContainer = smart.Page
                    .Locator("[data-testid='dropdown_list-options']:visible, [data-signature='names-list']:visible")
                    .Last;

                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                // 3. Выбираем нужный пункт по точному тексту
                // Здесь оставляем Exact = true, чтобы кликнуть именно туда, куда нужно
                var targetOption = optionsContainer
                    .Locator("[data-signature='dropdown_list-item'], [data-signature='checkbox-selector-wrapper']")
                    .GetByText(text, new() { Exact = true })
                    .Last;

                await targetOption.ClickAsync();

                // 4. ПРОВЕРКА: Простой ToContainText без Regex и без Exact
                // Он увидит 'ООО Альфа-Групп' внутри 'Сторона 1 *ООО Альфа-Групп' и будет счастлив
                await Assertions.Expect(smart.Locator).ToContainTextAsync(text);

                // 5. Ждем закрытия списка для стабильности
                await optionsContainer.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
            });
        }
    }
}
