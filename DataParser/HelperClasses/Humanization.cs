using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataParser.HelperClasses
{
    class Humanization
    {
        private static readonly Dictionary<string, string> Translit = new Dictionary<string, string>
        {
            ["й"] = "y", ["ц"] = "ts", ["у"] = "u", ["к"] = "k", ["е"] = "e",
            ["н"] = "n", ["г"] = "g", ["ш"] = "sh", ["щ"] = "shch", ["з"] = "z",
            ["х"] = "kh", ["ф"] = "f", ["ы"] = "y", ["в"] = "v", ["а"] = "a",
            ["п"] = "p", ["р"] = "r", ["о"] = "o", ["д"] = "d", ["ж"] = "zh",
            ["э"] = "e", ["я"] = "ya", ["ч"] = "ch", ["с"] = "s", ["м"] = "m",
            ["и"] = "i", ["т"] = "t", ["б"] = "b", ["ю"] = "yu", [" "] = "-",
            ["л"] = "l", ["ь"] = "", ["ъ"] = "",
        };

        public static string GetHumanLink(string text)
        {
            text = text.ToLower();
            text = Translit
                .Aggregate(text, (current, ch) => current.Replace(ch.Key, ch.Value));

            return Regex.Replace(text, @"[^\w-]", String.Empty);
        }
    }
}
