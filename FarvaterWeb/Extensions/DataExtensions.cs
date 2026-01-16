using System;

namespace FarvaterWeb.Extensions
{
    public static class DataExtensions
    {
        // Метод для получения уникального постфикса на основе времени (ддММггЧЧммсс)
        // Он даст строку типа "15011425" (15 января, 14:25)
        public static string GetUniquePostfix()
            => DateTime.Now.ToString("ddHHmm"); //если будут нужны секунды сделать "ddHHmmss"

        // Метод для генерации случайного числа заданной длины
        public static string GetRandomDigits(int length)
        {
            var random = new Random();
            string s = "";
            for (int i = 0; i < length; i++)
                s = string.Concat(s, random.Next(10).ToString());
            return s;
        }
    }
}