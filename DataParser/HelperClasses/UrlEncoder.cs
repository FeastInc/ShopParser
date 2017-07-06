using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.HelperClasses
{
    class UrlEncoder
    {
        public bool IsRussianLiteral(char literal)
        {
            return 'а' <= literal && literal <= 'я' || 'А' <= literal && literal <= 'Я';
        }

        public string UrlEncode(string str)
        {
            var a = IsRussianLiteral('a') ? char.Parse(WebUtility.UrlEncode('a'.ToString())) : 'a';
            return String.Empty;
        }
    }
}
