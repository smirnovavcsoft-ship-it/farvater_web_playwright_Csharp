using FarvaterWeb.Data;
using FarvaterWeb.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.TestData
{
    public class UserTestData
    {
        public static IEnumerable<object[]> GetUniversalUserCases()
        {
            var actor = CommonTestData.GetAdmin();
            var newUser = DataFactory.GenerateUserModel();
            var department = DataFactory.GenerateDepartmentModel();
            var position = DataFactory.GeneratePositionModel();
            var user = DataFactory.GenerateUserDetails(department.Name!, position.Description!);
            // Позитивный кейс - создание нового юзера
            yield return new object[] { actor, newUser, department, position, new UserDetails
                (
                user.LastName,
                user.FirstName,
                user.IdNumber,
                department.Name!,
                position.Description!,
                user.IsLeader,
                user.HasARightToSign,
                user.IsDomainUser,
                user.AuthenticationType,
                user.Login,
                user.Language,
                user.Phone,
                user.Email
                ),"success" };
            // Негативный кейс - попытка создать юзера с уже существующим логином
           /* yield return new object[] { actor, newUser, department, position, new UserDetails
                (
                user.LastName,
                user.FirstName,
                user.IdNumber,
                department.Name!,
                position.Description!,
                user.IsLeader,
                user.HasARightToSign,
                user.IsDomainUser,
                user.AuthenticationType,
                user.Login,
                user.Language,
                user.Phone,
                user.Email
                ), "failure" };*/


        }
    }
}
