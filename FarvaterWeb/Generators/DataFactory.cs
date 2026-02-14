using System;
using Bogus;
using FarvaterWeb.Data;

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

            return new UserModel
            {
                firstName = firstName,
                lastName = lastName,
                description = $"{lastName} {firstName}",
                login = FakerRu.Internet.UserName(lastName, firstName).Replace(".", "_"),
                mail = FakerRu.Internet.Email(lastName, firstName),
                phone = FakerRu.Phone.PhoneNumber("+79#########"),
                personnelNumber = FakerRu.Random.ReplaceNumbers("####"),
                isDisabled = false,
                isLeader = false,
                language = "ru"
            };
        }

        /// <summary>
        /// Генерирует модель контрагента (организации)
        /// </summary>
        public static CounterpartyModel GenerateCounterparty()
        {
            var companyName = FakerRu.Company.CompanyName();

            return new CounterpartyModel
            {
                // ИНН для юрлиц (10 цифр)
                inn = FakerRu.Random.ReplaceNumbers("##########"),
                // КПП (9 цифр)
                kpp = FakerRu.Random.ReplaceNumbers("#########"),
                // ОГРН (13 цифр)
                ogrn = FakerRu.Random.ReplaceNumbers("#############"),
                shorttitle = companyName,
                title = $"ООО {companyName}",
                address = FakerRu.Address.FullAddress(),
                phone = FakerRu.Phone.PhoneNumber("+7 (812) ###-##-##"),
                email = FakerRu.Internet.Email(),
                type = "LEGALENTITY_DEF"
            };
        }
    }
}
