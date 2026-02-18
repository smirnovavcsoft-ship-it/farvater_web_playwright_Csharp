using FarvaterWeb.Data;
using FarvaterWeb.Generators;
using FarvaterWeb.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.TestData
{
    public static class ContractTestData
    {
        public static IEnumerable<object[]> GetUniversalContractCases()
        {
            // Берем админа из общего файла
            var actor = CommonTestData.GetAdmin();
            // Генерируем обычного юзера
            var newUser = DataFactory.GenerateUser();

            var counterparty = DataFactory.GenerateCounterparty();
            // Кейс 1: Позитивный под админом
            yield return new object[] { actor, new ContractDetails("Контракт 1"
                , "Описание контракта", counterparty.ShortTitle, counterparty.ShortTitle
                , newUser.LastName!, newUser.FirstName!), "SUCCESS" };

            counterparty = DataFactory.GenerateCounterparty();
            // Кейс 2: Позитивный под обычным юзером
            yield return new object[] { newUser, new ContractDetails("Контракт 1"
                , "Описание контракта", counterparty.ShortTitle, counterparty.ShortTitle
                , newUser.LastName!, newUser.FirstName!), "SUCCESS" };

            counterparty = DataFactory.GenerateCounterparty();
            // Кейс 3: Негативный под админом (пустой предмет договора)
            yield return new object[] { newUser, new ContractDetails(""
                , "Описание контракта", counterparty.ShortTitle, counterparty.ShortTitle
                , newUser.LastName!, newUser.FirstName!), "SUCCESS" };

            counterparty = DataFactory.GenerateCounterparty();
            // Кейс 4: Негативный под админом (пустая сторона 1)
            yield return new object[] { newUser, new ContractDetails(""
                , "Описание контракта", "", counterparty.ShortTitle
                , newUser.LastName!, newUser.FirstName!), "SUCCESS" };

            counterparty = DataFactory.GenerateCounterparty();
            // Кейс 5: Негативный под админом (пустой предмет договора)
            yield return new object[] { newUser, new ContractDetails(""
                , "Описание контракта", counterparty.ShortTitle, ""
                , newUser.LastName!, newUser.FirstName!), "SUCCESS" };

        }
    }
}
