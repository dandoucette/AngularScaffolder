using System.Collections.Generic;

namespace AngularScaffolder.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Takes a pascal or camel cased string and splits that string
        /// into words based on the capital letters.
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>List of string</returns>
        public static List<string> ToWords(this string value)
        {
            int count = 0;
            int lastCount = 0;
            var words = new List<string>();

            foreach (char c in value)
            {
                if (c >= 65 && c <= 90)
                {
                    //capital letter
                    words.Add(value.Substring(lastCount, count - lastCount));
                    lastCount = count;
                }
                count++;
            }
            words.Add(value.Substring(lastCount));

            return words;
        }

        public static string ToCamelCase(this string value)
        {
            return $"{value.Substring(0, 1).ToLower()}{value.Substring(1)}";
        }
    }
}
