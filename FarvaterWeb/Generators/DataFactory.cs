using Bogus;
using FarvaterWeb.Data;
using FarvaterWeb.Extensions;
using System;

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

        public static ContractDetails GenerateContractDetails()
        {
            return new ContractDetails
            (
                ContractSubject: FakerRu.Company.CatchPhrase(),
                Party1Name: FakerRu.Company.CompanyName(),
                Party2Name: FakerRu.Company.CompanyName(),
                Cost: FakerRu.Finance.Amount(10000, 1000000).ToString("F2"),
                WithNDS: FakerRu.Finance.Amount(1000, 100000).ToString("F2"),
                TotalCost: FakerRu.Finance.Amount(11000, 1100000).ToString("F2")
            //ReferredAs1: "Сторона 1",
            //ReferredAs2: "Сторона 2"
            );
        }

        public static IncomeDocumentDetails GenerateIncomeDocumentDetails()
        {
            return new IncomeDocumentDetails
            (
                Summary: FakerRu.Lorem.Sentence(5, 10),
                SenderNumber: FakerRu.Random.Replace("###-###")
            );
        }
}
}