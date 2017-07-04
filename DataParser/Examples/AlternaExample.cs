using System;
using System.Collections.Generic;
using DataParser.HelperClasses;
using HtmlAgilityPack;

namespace DataParser.Examples
{
    public class AlternaExample
    {
        public static void Parse()
        {
            var alterna = new AlternaClass(
                blockExp:@"//div[contains(@class, ""level_2"")]/div[@class=""item""]",
                refProductExp:@"//div[@class=""uss_shop_name""]/a",
                singlePropertiesProduct: new Dictionary<string, Search<string>>
                {
                    ["Изображения"] = (node, o) => node
                        .SelectSingleNode(@"//a[@class=""enlarge_image_inside""]")
                        .Attributes["href"].Value,
                    ["Наименование"] = (node, o) => node
                        .SelectSingleNode(@"//h1")
                        .InnerText,
                    ["Цена"] = (node, o) => node
                        .SelectSingleNode(@"//div[@class=""uss_shop_price""]/span")
                        .InnerText,
                    ["Описание"] = (node, o) => node
                        .SelectSingleNode(@"//div[@class=""uss_shop_full_description""]")
                        .InnerHtml,
                    ["Габариты"] = (node, o) => node
                        .SelectSingleNode(@"//div[@class=""uss_shop_technical_data""]/div[@class=""uss_shop_description""]")
                        .InnerText,
                },
                propertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, o) => new string('!', (int)o.Args[0]) + node
                        .SelectSingleNode(node.XPath + @"//div[contains(@class, ""name"") and contains(@class, ""level_2"")]")
                        .InnerText.Trim()
                });
            var arguments = new ArgumentObject(url: "http://xn--80aaoxlrm3f.xn--p1ai/store/13488/",
                args: new object[] { 1 });
            var collection = alterna.GetProductOrCategory(arguments);
            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Мебель детская"}, isCategory: true)
            }.Extend(collection);
            Import.Write(path: "alterna.csv"
                , collection: collection
                , headers: Constants.WebAsystKeys
                , format: Constants.WebAsystFormatter);

        }
    }
}
