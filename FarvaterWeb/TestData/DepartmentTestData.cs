using FarvaterWeb.Data;
using FarvaterWeb.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.TestData
{
    public static class DepartmentTestData
    {
        public static IEnumerable<object[]> GetUniversalDepartmentCases()
        {
            var actor = CommonTestData.GetAdmin();
            //var newUser = DataFactory.GenerateUser();
            var department = DataFactory.GenerateDepartmentDetails();

            // Пример возврата одного тестового случая
            yield return new object[] { actor, new DepartmentDetails(department.Name, department.Code), "success" };
        }
    }
}