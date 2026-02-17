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
        inn = FakerRu.Random.ReplaceNumbers("##########"),
        kpp = FakerRu.Random.ReplaceNumbers("#########"),
        ogrn = FakerRu.Random.ReplaceNumbers("#############"),
        
        // Краткое название: "ТехноПром"
        shorttitle = brandName, 
        
        // Полное название: "ООО ТехноПром"
        title = $"ООО {brandName}", 
        
        address = FakerRu.Address.FullAddress(),
        phone = FakerRu.Phone.PhoneNumber("+7 (812) ###-##-##"),
        email = FakerRu.Internet.Email(),
        type = "LEGALENTITY_DEF"
    };
}
    }
}
