using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    class Import
    {
        public static IEnumerable<string> GetOrderedEnumerable(string[] order
            , Dictionary<string, string> properties
            , Func<string, string> format)
        {
            foreach (var key in order)
                yield return properties.ContainsKey(key) ? format(properties[key]) : String.Empty;
        }

        public static void Write(string path
            , IEnumerable<ProductCategoryObject> collection
            , string[] headers
            , Func<string, string> format = null)
        {
            format = format == null ? s => s : format;
            File.WriteAllText(path, string.Join(";", headers) + "\n", Encoding.Default);
            File.AppendAllLines(path
                , collection.Select(x => string.Join(";", GetOrderedEnumerable(headers, x.Properties, format))
                ), Encoding.Default);
        }
    }
}
