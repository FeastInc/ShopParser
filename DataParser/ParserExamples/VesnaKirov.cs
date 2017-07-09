using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DataParser.HelperClasses;

namespace DataParser.ParserExamples
{
    class VesnaKirov
    {
        public static void Parse()
        {
            var images = new HashSet<string>();
            var URL = @"http://www.vesna.kirov.ru";
            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//h1")
                    ?.InnerText ?? String.Empty,
                [@"""Код артикула"""] = (node, args) =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Delay");
                    return node
                        .SelectSingleNode(@".//*[@id='content']/div[2]/div/div/div[1]/div[2]/div[2]")
                        ?.InnerText?.Substring(9) ?? String.Empty;
                },
                ["Цена"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='content']//div[contains(@class, 'pro-roght-price')]")
                    ?.InnerText?.TrimEnd(new[] { '₽', ' ' }) ?? String.Empty,
                ["Описание"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='tabs']/div/div[1]")
                    ?.InnerHtml ?? String.Empty +
                    node
                    .SelectSingleNode(@".//*[@id='tabs']/div/div[2]")
                    ?.InnerHtml?.Replace("pro-info-list", String.Empty) ?? String.Empty,
                ["Валюта"] = (node, o) => "RUB",
                [@"""Доступен для заказа"""] = (node, o) => "1",
                [@"Статус"] = (node, o) => "1",
            };

            singlePropertiesProduct["Заголовок"] =
                (node, args) => singlePropertiesProduct["Наименование"](node, args);
            singlePropertiesProduct[@"""Ссылка на витрину"""] = (node, args) =>
                Humanization.GetHumanLink(singlePropertiesProduct["Наименование"](node, args));
            singlePropertiesProduct[@"""Краткое описание"""] =
                (node, args) => singlePropertiesProduct["Описание"](node, args).Split('.')[0];

            var parser = new LiquiMolyClass(
                isCategory: node => node
                    ._SelectNodes(@"//*[@class='pager']")
                    .Count != 0,
                findProducts: (node, args) => node
                    ._SelectNodes(@"//div[@class='catalog-item']/div[1]/a[1]")
                    .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value))
                    .ToArray(),
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => new string('!', (int)args.Args[0]) + node
                        .SelectSingleNode(@"//h1")
                        .InnerText,
                },
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@".//*[@id='content']/div[2]/div/div/div[1]/div[1]/p/a")
                        .Select(x => URL + x.Attributes["href"].Value)
                        .Where(x => images.Add(x))
                        .ToArray()
                },
                singlePropertiesProduct: singlePropertiesProduct,
                xPathPagination: (node, args) => node
                    ._SelectNodes(@".//*[@id='content']/div[2]/div/div/div[4]/a[not(contains(@class, 'active'))]")
                    .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value))
                    .ToArray()
                );
            var arguments = new ArgumentObject(url: URL,
                args: new object[] { 2 });
            var collection = parser.GetProductOrCategory(parser.GetLinks(args: arguments,
                url: URL,
                xPath: @".//*[@id='header']/div[2]/div/nav/ul/li[1]/div/ul/li/a"));
            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Temporary"}, isCategory: true),
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "!VesnaKirov"}, isCategory: true)
            }.Extend(collection);
            Import.Write(path: "VesnaKirov.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
