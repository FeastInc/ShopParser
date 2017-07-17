using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataParser.HelperClasses;

namespace DataParser.ParserExamples
{
    class ValdaExample
    {
        public static void Parse()
        {
            var URL = @"http://valda.ru";
            var singlePropertiesProduct = new Dictionary <string, Search<string>>
            {
                ["Цена"] = (node, args) => node
                        .SelectSingleNode(@"//div[@class='add2cart']/span[2]")
                        ?.Attributes["data-price"]?.Value ??
                        node.SelectSingleNode(@"//div[@class='add2cart']/span[3]")
                        .Attributes["data-price"].Value,
                [@"""Код артикула"""] = (node, args) => node
                    .SelectSingleNode(@"//div[@itemprop]/*[contains(@class, 'hint')]")
                    ?.InnerText ?? string.Empty,
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//article/h1/span")
                    .InnerText,
                ["Описание"] = (node, args) => (node
                    .SelectSingleNode(@".//*[@id='product-description']")
                    ?.InnerHtml ?? string.Empty) +
                    (node.SelectSingleNode(@".//*[@id='cart-form']/p")
                    ?.InnerHtml ?? string.Empty) + 
                    (node.SelectSingleNode(@".//*[@id='cart-form']/table")
                    ?.InnerHtml ?? string.Empty),
                ["Валюта"] = (node, o) => "RUB",
                [@"""Доступен для заказа"""] = (node, o) => "1",
                [@"Статус"] = (node, o) => "1",
                [@"""Зачеркнутая цена"""] = (node, args) => node
                    .SelectSingleNode(@"//span[@class='compare-at-price nowrap']")
                    ?.InnerText?.Replace(" ", string.Empty) ?? string.Empty,
                //[@"""Возраст детей"""] = (node, args) =>
                //{
                //    return Regex.Match(node.InnerText, @"Возраст.*\s+\w+", RegexOptions.IgnoreCase).Value;
                //}
            };
            singlePropertiesProduct["Заголовок"] =
                (node, args) => singlePropertiesProduct["Наименование"](node, args);
            singlePropertiesProduct[@"""Ссылка на витрину"""] = (node, args) =>
                Humanization.GetHumanLink(singlePropertiesProduct["Наименование"](node, args));
            var parser = new LiquiMolyClass(
                isCategory: node => node
                    ._SelectNodes(@"//*[@id='product-list']")
                    .Count != 0,
                findProducts: (node, args) => node
                    ._SelectNodes(@"//*[@id='product-list']/ul/li/a")
                    .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value))
                    .ToArray(),
                singlePropertiesProduct: singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) =>
                    {

                       var result =  node
                            ._SelectNodes(@"//div[@id='product-gallery']/div/a")
                            .Select(x => URL + x.Attributes["href"].Value)
                            .ToArray();
                        return result.Length != 0
                            ? result
                            : new [] { URL + node.SelectSingleNode(@".//*[@id='product-image']")
                                  .Attributes["src"].Value };
                    }
                },
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => new string('!', (int)args.Args[0]) + node
                        .SelectSingleNode(@".//*[@id='page-content']/h1")
                        .InnerText.Trim()
                }
                );
            var argument = new ArgumentObject(
                url: URL,
                //prefix: @"http://oksva-tm.ru/catalog/15",
                args: new object[] { 2 });

            var collection =
                parser.GetProductOrCategory(parser.GetLinks(argument,
                    @".//*[@id='page-content']/div[1]/ul/li/a",
                    prefix: URL));
            //parser.GetProductOrCategory(argument);
            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Temporary"}, isCategory: true),
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "!valda"}, isCategory: true)
            }.Extend(collection);
            Import.Write(path: "valda.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
