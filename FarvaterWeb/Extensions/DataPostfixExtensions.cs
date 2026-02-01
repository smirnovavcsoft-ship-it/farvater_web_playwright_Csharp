using System;

namespace FarvaterWeb.Extensions
{
    public static class DataPostfixExtensions
    {
        public static string GetUniquePostfix()
            => DateTime.Now.ToString("ddHHmm");      

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