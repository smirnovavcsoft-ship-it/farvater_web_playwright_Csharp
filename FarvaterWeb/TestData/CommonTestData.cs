using FarvaterWeb.Data;
using FarvaterWeb.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.TestData
{
    public static class CommonTestData
    {
        // Тот самый существующий админ
        public static UserModel GetAdmin() => new UserModel
        {
            Login = "SYSADMIN",
            // Password = "",
           
        };

        // Базовый пользователь из фабрики (всегда новый)
        public static UserModel GetRegularUser() => DataFactory.GenerateUser();

        // Руководитель (ГИП) на базе генерации
        public static UserModel GetGip()
        {
            var user = DataFactory.GenerateUser();
            user.IsLeader = true; // Предполагаем, что поле называется так
            return user;
        }
    }
}
