using FarvaterWeb.Data;
using FarvaterWeb.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.TestData
{
    public static class GroupTestData
    {
        public static IEnumerable<object[]> GetUniversalGroupCases()
        {
            var actor = CommonTestData.GetAdmin();
            var newUser = DataFactory.GenerateUserModel();
            var group = DataFactory.GenerateGroupDetails(newUser.LastName!,newUser.FirstName!);

            // Пример возврата одного тестового случая
            yield return new object[] { actor, newUser, new GroupDetails(group.GroupName, group.Responsible, group.IsAdmin, group.IsGip, group.IsArchive, group.IsContracts, group.IsOrd), "success" };
        }
    }
}
