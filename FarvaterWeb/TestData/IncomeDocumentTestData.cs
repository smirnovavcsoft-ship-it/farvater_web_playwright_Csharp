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
    public static class IncomeDocumentTestData
    {
        public static IEnumerable<object[]> GetUniversalIncomeDocumentCases()
        {

            var actor = CommonTestData.GetAdmin();

            var newUser = DataFactory.GenerateUser();

            var counterparty = DataFactory.GenerateCounterparty();

            var incomeDocument = DataFactory.GenerateIncomeDocumentDetails();
            //string content = DataFactory.FakerRu.Lorem.Sentence(5, 10);
            //string number = DataFactory.FakerRu.Random.Replace("####");
            //return new IncomeDocumentDetails(content, number);
            yield return new object[] { actor, counterparty, new IncomeDocumentDetails("Письмо", incomeDocument.PlanningResponseDate, incomeDocument.Summary, incomeDocument.SenderNumber, counterparty.ShortTitle, incomeDocument.FromDate), "Успешно" };
        }
    }
}
