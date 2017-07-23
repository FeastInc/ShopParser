using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DataParser.ParserExamples
{
    static class MasterasExample
    {
        public static void Parse()
        {
            var URL = @"https://masteras.ru";

            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//*[@id='product-top']/div[2]/h1")
                    .InnerText,
                [@"""Код артикула"""] = (node, args) => "SER-" + node
                    .SelectSingleNode(@"//*[@class='sku']/span")
                    .InnerText
                    .Trim(),
                ["Цена"] = (node, args) => node
                    .SelectSingleNode(@"//*[@id='product-top']/div[2]/div[1]/p/span")
                    ?.InnerText
                    .Replace(" ", string.Empty)??string.Empty,
                ["Описание"] = (node, args) => node
                   .SelectSingleNode(@"//*[@class='hide-me closed']")
                   ?.InnerText ?? string.Empty,
                ["Валюта"] = (node, o) => "RUB",
                [@"""Доступен для заказа"""] = (node, o) => "1",
                [@"Статус"] = (node, o) => "1",
            };

            singlePropertiesProduct["Заголовок"] =
                (node, args) => singlePropertiesProduct["Наименование"](node, args);
            singlePropertiesProduct[@"""Ссылка на витрину"""] = (node, args) =>
                Humanization.GetHumanLink(singlePropertiesProduct["Наименование"](node, args)
                                          + "-" + singlePropertiesProduct[@"""Код артикула"""](node, args));

            var parser = new LiquiMolyClass(
                isCategory: node => node
                    ._SelectNodes(@"//*[@class='sku']/span")
                    .Count == 0,
                findProducts: (node, args) => 
                {
                     var prods = node
                    .SelectNodes(@"//a[@class='product-title']")
                    .Select(x => new ArgumentObject(x.Attributes["href"].Value))
                    .ToArray();

                    return (int)args.Args[0] > 1 ? prods : new ArgumentObject[0];
                },
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) =>
                    {
                        var a = node
                        .SelectSingleNode(@"//*[@id='cat-description']/h2")
                        ?.InnerText.Trim() ?? string.Empty;

                        var b = node
                        .SelectSingleNode(@"//*[@id='categories']/h2")
                        ?.InnerText.Trim() ?? string.Empty;

                        return new string('!', (int)args.Args[0]) + (a == string.Empty ? b : a);
                    }
                },
                singlePropertiesProduct: singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//*[@id='photos']/div/a/img")
                        .Select(x => x.Attributes["src"].Value)
                        .ToArray()
                },
                xPathPagination: (node, args) =>
                {
                    var number = int.Parse(node
                      .SelectSingleNode(@"//*[@class='page-numbers']/li[last() -1]/a")
                      ?.InnerText??"1");

                    return Enumerable.Range(2, number - 1)
                        .Select(x => args.Url + $"page/{x}/")
                        .Select(x => new ArgumentObject(x))
                        .ToArray();
                },
                encoding: Encoding.UTF8
            );

            var argument = new ArgumentObject(
               url: @"https://masteras.ru/shop/",
               args: new object[] { 2 });

            var collection =
            parser.GetProductOrCategory(parser.GetLinks(argument,
                @"//*[@class='descr']/../../a",
                prefix: ""));

            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Temporary2"}, isCategory: true),
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "!Masteras"}, isCategory: true)
            }.Extend(collection);

            //collection = JoinerArticles.JoinInOrderEnumerable(collection, "Наименование",
            //    productFieldsForPluralProp: new[] { "Изображения" },
            //    productFieldsForSingleProp: new[] { "Описание" });

            Import.Write(path: "../../../CSV/masteras.csv",
               collection: collection.ToArray(),
               headers: Constants.WebAsystKeys,
               format: Constants.WebAsystFormatter);
        }
    }
}
