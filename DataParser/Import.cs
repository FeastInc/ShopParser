using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataParser.HelperClasses;

namespace DataParser
{
    class Import
    {
        public static IEnumerable<string> GetOrderedEnumerable(string[] order,
            ProductCategoryObject obj,
            Dictionary<string, int> countsPluralProperties)
        {
            foreach (var key in order)
                yield return obj.SingleProperties.ContainsKey(key) 
                    ? obj.SingleProperties[key] 
                    : String.Empty;
            foreach (var property in obj.PluralProperties)
            {
                var delta = countsPluralProperties[property.Key] - property.Value.Length;
                foreach (var s in property.Value)
                    yield return s;
                for (int i = 0; i < delta; i++)
                    yield return String.Empty;
            }
        }

        public static void Write(string path
            , ProductCategoryObject[] collection
            , string[] headers
            , Func<string, string> format = null)
        {

            format = format ?? (s => s);
            var counts = collection
                .SelectMany(x => x.PluralProperties
                    .Select(y => Tuple.Create(y.Key, y.Value.Length)))
                .GroupBy(x => x.Item1)
                .ToDictionary(x => x.Key, x => x.Max(y => y.Item2));

            var result = collection
                .Select(x => GetOrderedEnumerable(
                        order: headers,
                        obj: x,
                        countsPluralProperties: counts))
                .Select(x => x.Select(y => format(y)))
                .Select(x => string.Join(";", x));

            var headersExtended = headers.Extend(counts.SelectMany(x => Enumerable
                .Range(0, x.Value)
                .Select(z => x.Key)));
            File.WriteAllBytes(
                path: path,
                bytes: Constants.BOM);
            File.WriteAllText(
                path: path, 
                contents: string.Join(";", headersExtended) + "\n", 
                encoding: Encoding.UTF8);
            File.AppendAllLines(
                path: path, 
                contents: result, 
                encoding: Encoding.UTF8);
        }
    }
}
