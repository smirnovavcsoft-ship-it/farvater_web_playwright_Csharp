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
        public static IncomeDocumentDetails GenerateIncomeDocumentDetails()
        {
            string content = DataFactory.FakerRu.Lorem.Sentence(5, 10);
            string number = DataFactory.FakerRu.Random.Replace("####");
            return new IncomeDocumentDetails(content, number);
        }
    }
}
