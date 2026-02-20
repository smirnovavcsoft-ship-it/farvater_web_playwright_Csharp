using Bogus;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using Microsoft.Playwright;
using RazorEngine;
using System;
using System.IO.Compression;

namespace FarvaterWeb.Generators
{
    public static class DataFactory
    {
        private static readonly Faker FakerRu = new("ru");

        /// <summary>
        /// Генерирует модель пользователя для API
        /// </summary>
        public static UserModel GenerateUser()
        {
            var gender = FakerRu.PickRandom<Bogus.DataSets.Name.Gender>();
            var firstName = FakerRu.Name.FirstName(gender);
            var lastName = FakerRu.Name.LastName(gender);
            string postfix = DataPostfixExtensions.GetUniquePostfix();

            return new UserModel
            {
                FirstName = firstName,
                LastName = lastName,
                Description = $"{lastName} {firstName}",
                Login = FakerRu.Internet.UserName(lastName, $"{firstName}{postfix}").Replace(".", "_"),
                Mail = FakerRu.Internet.Email(lastName, firstName),
                Phone = FakerRu.Phone.PhoneNumber("+79#########"),
                PersonnelNumber = FakerRu.Random.ReplaceNumbers("####"),
                IsDisabled = false,
                IsLeader = false,
                Language = "ru"
            };
        }

        /// <summary>
        /// Генерирует модель контрагента (организации)
        /// </summary>
        public static CounterpartyModel GenerateCounterparty()
        {
            // Генерируем "голый" бренд без ООО/ИП
            // Можно использовать CatchPhrase для более креативных названий
            //var brandName = FakerRu.Company.CatchPhrase(); 
            var brandName = FakerRu.Company.CompanyName();

            return new CounterpartyModel
            {
                Inn = FakerRu.Random.ReplaceNumbers("##########"),
                Kpp = FakerRu.Random.ReplaceNumbers("#########"),
                Ogrn = FakerRu.Random.ReplaceNumbers("#############"),

                // Краткое название: "ТехноПром"
                ShortTitle = brandName,

                // Полное название: "ООО ТехноПром"
                FullTitle = $"ООО {brandName}",

                Address = FakerRu.Address.FullAddress(),
                Phone = FakerRu.Phone.PhoneNumber("+7 (812) ###-##-##"),
                Email = FakerRu.Internet.Email(),
                Type = "LEGALENTITY_DEF"
            };
        }

        public static ContractDetails GenerateContractDetails(string shortTitle = "")
        {

            return new ContractDetails
            (
                ContractSubject: "Предмет договора",
                ContractType: "Договор подряда",
                Party1: shortTitle,
                Party2: shortTitle,
                Party1Name: "Сторона 1",
                Party2Name: "Сторона 2",
                Cost: FakerRu.Finance.Amount(10000, 1000000).ToString("F2"),
                WithNDS: FakerRu.Finance.Amount(1000, 100000).ToString("F2"),
                TotalCost: FakerRu.Finance.Amount(11000, 1100000).ToString("F2")
            //ReferredAs1: "Сторона 1",
            //ReferredAs2: "Сторона 2"
            );
        }

        public static IncomeDocumentDetails GenerateIncomeDocumentDetails(string shortTitle = "")
        {
            return new IncomeDocumentDetails
            (
                DocumentType: "Письмо",
                PlanningResponseDate: DateTime.Now.AddDays(FakerRu.Random.Int(1, 30)),
                // Project: "Проект 1",
                Summary: FakerRu.Lorem.Sentence(5, 10),
                SenderNumber: FakerRu.Random.Replace("###-###"),
                Sender: shortTitle,
                FromDate: DateTime.Now.AddDays(-FakerRu.Random.Int(1, 30))
            );
        }

        public static OutcomeDocumentDetails GenerateOutcomeDocumentDetails(string lastName, string firstName, string shortTitle = "")
        {
            return new OutcomeDocumentDetails
            (
                Summary: FakerRu.Lorem.Sentence(5, 10),
                Resipient: shortTitle,
                Performer: $"{lastName} {firstName[0]}."

            );
        }

        public static GroupDetails GenerateGroupDetails(string lastName, string firstName)
        {
            return new GroupDetails
            (
                GroupName: "Тестировщики",
                Responsible: $"{lastName} {firstName[0]}.",
                IsAdmin: true,
                IsGip: true,
                IsArchive: true,
                IsContracts: true,
                IsOrd: true
            );
        }

        public static DepartmentDetails GenerateDepartmentDetails()
        {
            var name = "Отдел Тестирования";

            // 1. Разбиваем строку по пробелам
            // 2. Убираем пустые элементы (если случайно поставили два пробела)
            // 3. Берем первую букву каждого слова
            var code = string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(word => word[0]));
            return new DepartmentDetails
            (
                Name: name,
                Code: code
            );
        }

        public static UserDetails GenerateUserDetails(string department, string position)
        {
            var gender = FakerRu.PickRandom<Bogus.DataSets.Name.Gender>();
            var firstName = FakerRu.Name.FirstName(gender);
            var lastName = FakerRu.Name.LastName(gender);
            string postfix = DataPostfixExtensions.GetUniquePostfix();

            return new UserDetails
            (
                LastName: lastName,
                FirstName: firstName,
                IDnumber: FakerRu.Random.ReplaceNumbers("##########"),
                Department: department,
                Position: position,
                IsLeader: false,
                HasARightToSign: false,
                IsDomainUser: true,
                AuthenticationType: "Аутентицификация TDMS",
                Login: FakerRu.Internet.UserName(lastName, $"{firstName}{postfix}").Replace(".", "_"),
                Language: "Русский",
                Phone: FakerRu.Phone.PhoneNumber("+79#########"),
                Email: FakerRu.Internet.Email(lastName, firstName)

            );
        }

        public static ProjectModel GenerateProjectModel()
        {
            var projectName = FakerRu.Company.CatchPhrase();
            return new ProjectModel
            {
                Code = FakerRu.Random.Replace("PRJ-####"),
                Title = projectName,
                ProjectsObject = projectName
            };






        }
    }
}