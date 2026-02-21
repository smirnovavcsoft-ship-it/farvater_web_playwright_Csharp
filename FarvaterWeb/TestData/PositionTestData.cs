using FarvaterWeb.Data;
using FarvaterWeb.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.TestData
{
    public static class PositionTestData
    {
        
        public static IEnumerable<object[]> GetUniversalPositionCases()
        {
            var actor = CommonTestData.GetAdmin();

            var newUser = DataFactory.GenerateUserModel();

            var position = DataFactory.GeneratePositionDetails();
            // Здесь можно добавить генерацию данных для тестов, если это необходимо
            // Например, можно создать несколько различных наименований должностей
            yield return new object[] { "Тестовая должность" };
            
        }
    }
}
