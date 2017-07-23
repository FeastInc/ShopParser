using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using DataParser.HelperClasses;

namespace DataParser.ParserExamples
{
    class VesnaKirov
    {
        public static void Parse()
        {
            var random = new Random();
            var URL = @"http://www.vesna.kirov.ru";
            var suffix = @"?page_count=1000&sort=PROPERTY_IS_AVAILABLE|DESC&PAGEN_1=13";
            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//h1")
                    ?.InnerText ?? string.Empty,
                [@"""Код артикула"""] = (node, args) =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Delay");
                    return "VS-" + node
                        .SelectSingleNode(@".//*[@id='content']/div[2]/div/div/div[1]/div[2]/div[2]")
                        ?.InnerText?.Substring(9) ?? string.Empty;
                },
                ["Цена"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='content']//div[contains(@class, 'pro-roght-price')]")
                    ?.InnerText?.TrimEnd(new[] { '₽', ' ' }) ?? string.Empty,
                ["Описание"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='tabs']/div/div[1]")
                    ?.InnerHtml ?? string.Empty +
                    node
                    .SelectSingleNode(@".//*[@id='tabs']/div/div[2]")
                    ?.InnerHtml?.Replace("pro-info-list", string.Empty) ?? string.Empty,
                ["Валюта"] = (node, o) => "RUB",
                [@"""Доступен для заказа"""] = (node, o) => "1",
                [@"Статус"] = (node, o) => "1",
            };

            singlePropertiesProduct["Заголовок"] =
                (node, args) => singlePropertiesProduct["Наименование"](node, args);
            singlePropertiesProduct[@"""Ссылка на витрину"""] = (node, args) =>
                Humanization.GetHumanLink(singlePropertiesProduct["Наименование"](node, args) + "-VS-" + random.Next());

            var parser = new LiquiMolyClass(
                isCategory: node =>node
                        ._SelectNodes(@"//span[contains(text(), 'Сортировка')]")
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
                    ["Изображения"] = (node, args) =>node
                            ._SelectNodes(@"//*[contains(@class,'pic-default')]/div/a/img")
                            .Select(x => URL + x.Attributes["data-zoom-image"].Value)
                            .ToArray()
                },
                singlePropertiesProduct: singlePropertiesProduct
                );
            var arguments = new ArgumentObject(
                url: URL,
                //url: @"http://vesna.kirov.ru/catalog/igrushki-iz-pvh/" + suffix,
                args: new object[] { 2 });
            var collection = parser.GetProductOrCategory(parser.GetLinks(args: arguments,
                prefix: URL,
                xPath: @".//*[@id='header']/div[2]/div/nav/ul/li[1]/div/ul/li/a",
                suffix: suffix));
            //var collection = parser.GetProductOrCategory(arguments);
            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Temporary2"}, isCategory: true),
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "!VesnaKirov"}, isCategory: true)
            }.Extend(collection);
            Import.Write(path: @"..\..\..\CSV\VesnaKirov.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
