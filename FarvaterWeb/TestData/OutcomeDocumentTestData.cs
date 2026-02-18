using FarvaterWeb.Data;
using FarvaterWeb.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.TestData
{
    public static class OutcomeDocumentTestData
    {
        public static IEnumerable<object[]> GetUniversalContractCases()
        {

            var actor = CommonTestData.GetAdmin();

            var newUser = DataFactory.GenerateUser();

            var counterparty = DataFactory.GenerateCounterparty();

            var outcomeDocument = DataFactory.GenerateOutcomeDocumentDetails(newUser.LastName, newUser.FirstName);
            
            yield return new object[] { actor, newUser, counterparty, new OutcomeDocumentDetails(outcomeDocument.Summary, counterparty.ShortTitle, outcomeDocument.Performer), "Успешно" };
        }
    }
}
