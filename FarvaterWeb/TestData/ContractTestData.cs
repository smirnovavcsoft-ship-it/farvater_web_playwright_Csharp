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
            var newUser = DataFactory.GenerateUserModel();

            var counterparty = DataFactory.GenerateCounterpartyModel();

            var contract = DataFactory.GenerateContractDetails();
            // Позитивный под админом
            yield return new object[] { actor, counterparty, new ContractDetails(contract.ContractSubject, contract.ContractType
                , counterparty.ShortTitle, counterparty.ShortTitle, contract.Party1Name, contract.Party2Name, contract.Cost, contract.WithNDS, contract.TotalCost), "SUCCESS" };

            /*counterparty = DataFactory.GenerateCounterparty();
            // Позитивный под обычным юзером
            yield return new object[] { newUser, counterparty, new ContractDetails(contract.ContractSubject, contract.ContractType
                , counterparty.ShortTitle, counterparty.ShortTitle, contract.Party1Name, contract.Party2Name, contract.Cost, contract.WithNDS, contract.TotalCost), "SUCCESS" };

            counterparty = DataFactory.GenerateCounterparty();
            // Негативные с пропуском обязательных полей 
            yield return new object[] { actor, counterparty, new ContractDetails("", contract.ContractType
                , counterparty.ShortTitle, counterparty.ShortTitle, contract.Party1Name, contract.Party2Name, contract.Cost, contract.WithNDS, contract.TotalCost), "SUCCESS" };

            counterparty = DataFactory.GenerateCounterparty();
           
            yield return new object[] { actor, counterparty, new ContractDetails(contract.ContractSubject, ""
                , counterparty.ShortTitle, counterparty.ShortTitle, contract.Party1Name, contract.Party2Name, contract.Cost, contract.WithNDS, contract.TotalCost), "SUCCESS" };

            counterparty = DataFactory.GenerateCounterparty();
            
            yield return new object[] { actor, counterparty, new ContractDetails(contract.ContractSubject, contract.ContractType
                , "", counterparty.ShortTitle, contract.Party1Name, contract.Party2Name, contract.Cost, contract.WithNDS, contract.TotalCost), "SUCCESS" };

            counterparty = DataFactory.GenerateCounterparty();
            yield return new object[] { actor, counterparty, new ContractDetails(contract.ContractSubject, contract.ContractType
                , counterparty.ShortTitle, "", contract.Party1Name, contract.Party2Name, contract.Cost, contract.WithNDS, contract.TotalCost), "SUCCESS" };*/
        }
    }
}
