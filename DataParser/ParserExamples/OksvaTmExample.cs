using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class OksvaTmExample
    {
        public static void Parse()
        {
            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Цена"] = (node, args) => node
                    ?.SelectSingleNode(@".//*[contains(@id,'node')]/div[1]/div[2]/span[2]")
                    ?.InnerText?.Replace("р.", string.Empty).Replace(" ", string.Empty) ?? string.Empty,
                ["Описание"] = (node, args) => node
                    ?.SelectSingleNode(@".//*[contains(@id,'node')]/div[1]/div[5]/div/div/p")
                    ?.InnerHtml ?? string.Empty,
                ["Размеры"] = (node, args) => node
                    ?.SelectSingleNode(@".//*[contains(@id,'node')]/div[1]/div[9]/span[2]")
                    ?.InnerText ?? string.Empty,
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//h1")
                    .InnerText.Trim(),
                ["Валюта"] = (node, o) => "RUB",
                [@"""Доступен для заказа"""] = (node, o) => "1",
                [@"Статус"] = (node, o) => "1",
            };

            singlePropertiesProduct["Заголовок"] =
                (node, args) => singlePropertiesProduct["Наименование"](node, args);
            singlePropertiesProduct[@"""Ссылка на витрину"""] = (node, args) =>
                Humanization.GetHumanLink(singlePropertiesProduct["Наименование"](node, args));

            var parser = new LiquiMolyClass(
                isCategory: node => node
                               ._SelectNodes(@".//*[contains(@id,'node')]")
                               .Count == 0,
                findProducts: (node, args) =>node
                        ._SelectNodes(@"//table/tbody/tr/td[2]/a")
                        .Select(x => new ArgumentObject(@"http://oksva-tm.ru" + x.Attributes["href"].Value))
                        .ToArray(),
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => new string('!', (int)args.Args[0]) + node
                        .SelectSingleNode(@"//*[@id='page-title']")
                        .InnerText.Trim()
                },
                singlePropertiesProduct: singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//*[contains(@id,'node')]//img")
                        .Select(x => x.Attributes["src"].Value)
                        .ToArray(),
                },
                xPathPagination: (node, args) => node
                    ._SelectNodes(@".//*[@id='block-system-main']/div/div[2]/div[2]/ul/li[position() < last() - 1]/a")
                    .Select(x => new ArgumentObject(
                        WebUtility.HtmlDecode(@"http://oksva-tm.ru" + x.Attributes["href"].Value)))
                    .ToArray()
                );
            var argument = new ArgumentObject(
                url: @"http://oksva-tm.ru/catalog/",
                //url: @"http://oksva-tm.ru/catalog/15",
                args: new object[] { 2 });

            var collection =
            parser.GetProductOrCategory(parser.GetLinks(argument,
                @".//*[@id='block-system-main']/div/div[1]/div/table/tbody/tr/td/div/span/a",
                @"http://oksva-tm.ru"));
            //parser.GetProductOrCategory(argument);
            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Temporary"}, isCategory: true),
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "!Оксва-тм"}, isCategory: true)
            }.Extend(collection);
            Import.Write(path: "oksvatm.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
