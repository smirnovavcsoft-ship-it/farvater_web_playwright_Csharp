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
    public static class NoteTestData
    {
        public static IEnumerable<object[]> GetUniversalNoteCases()
        {
            // Берем админа из общего файла
            var actor = CommonTestData.GetAdmin();
            // Генерируем обычного юзера
            var newUser = DataFactory.GenerateUser();

            // Кейс 1: Позитивный под админом
            yield return new object[] { actor, newUser, new NoteDetails("Служебная", "Тема записки", "Содержание записки", newUser.LastName!, newUser.FirstName!), "SUCCESS" };


           /* // Кейс 2: Негативный под админом (пустая тема)
            newUser = DataFactory.GenerateUser();
            yield return new object[] { actor, newUser, new NoteDetails("Служебная", "", "Содержание записки", newUser.LastName!, newUser.FirstName!), "Поле 'Тема' обязательно" };

            // Кейс 3: Негативный под админом (пустое содержание)
            newUser = DataFactory.GenerateUser();
            yield return new object[] { actor, newUser, new NoteDetails("Служебная", "Тема записки", "", newUser.LastName!, newUser.FirstName!), "Поле 'Содержание' обязательно" };

            // Кейс 4: Позитивный под обычным юзером
            newUser = DataFactory.GenerateUser();
            yield return new object[] { newUser, newUser, new NoteDetails("Служебная", "Тема записки", "Содержание записки", newUser.LastName!, newUser.FirstName!), "SUCCESS" };

            // Кейс 4: Негативный (если юзеру запрещено что-то)
            // yield return new object[] { restrictedUser, new NoteDetails("Т", "Т"), "FORBIDDEN" };*/
        }
    }
}
