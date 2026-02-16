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
