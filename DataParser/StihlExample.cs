using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataParser.HelperClasses;
using MySql.Data.MySqlClient;

namespace DataParser
{
    class StihlExample
    {
        private static string[] GetImages(string text)
        {
            var match = Regex.Match(text, @"original"";s:\d+:""(/.*?)""");
            return new [] { match.Groups[1].Value };
        }

        public static void Parse()
        {
            var url = @"http://xn----itbwjj6a.xn--p1ai";
            var connection = new DbConnector("root", "n1k1t0s28mysql", "192.168.0.104", "u24223");
            var collection = connection.GetProductCategoryObjects(
                query: @"SELECT f.name, f.code, f.text, f.comment, f.img, s.title, s.code, s.img, " +
                       @"s.comment, s.har, s.text, s.price FROM mp_catalog_articles as f, " + 
                       @"mp_catalog_items as s WHERE f.id=s.parent ORDER BY f.name",
                singlePropertiesProductFunc: new Dictionary<string, Func<MySqlDataReader, string>>
                {
                    ["Наименование"] = reader => reader.GetString(5).Trim(),
                    [@"""Ссылка на витрину"""] = reader => reader.GetString(6),
                    ["Описание"] = reader => reader.GetString(10) + reader.GetString(9),
                    [@"""Краткое описание"""] = reader => reader.GetString(8),
                    [@"Цена"] = reader => reader.GetString(11),
                    ["Валюта"] = reader => "RUB",
                    [@"""Доступен для заказа"""] = reader => "1",
                    [@"Статус"] = reader => "1",
                },
                pluralPropertiesProductFunc: new Dictionary<string, Func<MySqlDataReader, string[]>>
                {
                    [@"Изображения"] = reader => GetImages(reader.GetString(7))
                        .Select(x => url + x)
                        .ToArray()
                },
                singlePropertiesCategoryFunc: new Dictionary<string, Func<MySqlDataReader, string>>
                {
                    ["Наименование"] = reader => '!' + reader.GetString(0).Trim(),
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
            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "spec-m"}, isCategory: true),
            }.Extend(collection);
            Import.Write(path: "../../../CSV/spec-m.csv",
                collection: collection
                    .Distinct(new ProductEqualityComparer(o => o.SingleProperties["Наименование"]))
                    .ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
