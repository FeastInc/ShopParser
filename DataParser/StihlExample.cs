using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DataParser
{
    class StihlExample
    {
        private static string[] GetImages(string text)
        {
            var matches = Regex.Matches(text, @"""(/.*?)""");
            var result = new List<string>();
            for (var i = 0; i < matches.Count; i++)
            {
                result.Add(matches[i].Groups[1].Value);
            }
            return result.ToArray();
        }

        public static void Parse()
        {
            var url = @"http://xn----itbwjj6a.xn--p1ai";
            var connection = new DbConnector("root", "n1k1t0s28mysql", "192.168.0.104", "u24223");
            var queryCategory = @"SELECT * FROM mp_catalog_articles where parent={0}";
            var queryProduct = @"SELECT * FROM mp_catalog_items WHERE parent={0}";
            var setIds = new HashSet<int>();
            var collection = connection.GetProductCategoryObjects(
                query: @"SELECT f.name, f.code, f.text, f.comment, f.img, s.title, s.code, s.img, " +
                       @"s.comment, s.har, s.text, s.price FROM mp_catalog_articles as f, " + 
                       @"mp_catalog_items as s WHERE f.id=s.parent",
                singlePropertiesProductFunc: new Dictionary<string, Func<MySqlDataReader, string>>
                {
                    ["Наименование"] = reader => reader.GetString(5),
                    [@"""Ссылка на витрину"""] = reader => reader.GetString(6),
                    ["Описание"] = reader => reader.GetString(10) + reader.GetString(9),
                    [@"""Краткое описание"""] = reader => reader.GetString(8),
                    [@"Цена"] = reader => reader.GetString(11)
                },
                pluralPropertiesProductFunc: new Dictionary<string, Func<MySqlDataReader, string[]>>
                {
                    [@"Изображения"] = reader => GetImages(reader.GetString(7))
                        .Select(x => url + x)
                        .ToArray()
                },
                singlePropertiesCategoryFunc: new Dictionary<string, Func<MySqlDataReader, string>>
                {
                    ["Наименование"] = reader => reader.GetString(0),
                    [@"""Ссылка на витрину"""] = reader => reader.GetString(1),
                    ["Описание"] = reader => reader.GetString(2),
                    [@"""Краткое описание"""] = reader => reader.GetString(3),
                },
                pluralPropertiesCategoryFunc: new Dictionary<string, Func<MySqlDataReader, string[]>>
                {
                    [@"Изображения"] = reader => GetImages(reader.GetString(4))
                        .Select(x => url + x)
                        .ToArray()
                });
            Import.Write(path: "../../../CSV/spec-m.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
