using System.Linq;
using System.Net;

namespace DataParser
{
    public class ArgumentObject
    {
        public string Url;
        public object[] Args;

        public ArgumentObject(string url, object[] args = null)
        {
            Url = WebUtility.HtmlDecode(url);
            Args = args?.ToArray();
        }

        public override string ToString()
        {
            return $"URL: {Url}\n" +
                   $"Arguments: {{{string.Join(", ", Args??new object[0])}}}\n" +
                   $"{new string('-', 10)}\n";
        }
    }
}
