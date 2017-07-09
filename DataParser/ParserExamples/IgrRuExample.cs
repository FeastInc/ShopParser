using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DataParser.DataExtractorExamples;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class IgrRuExample
    {
        public static void Parse()
        {
            var mainUrl = @"http://igr.ru/";
            var addedUrl = @"http://igr.ru/cat.php?pgsize=1000&sort=1&pgsort=1&days=9000&rub=207&prub=";

            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//h2")
                    .InnerText,
                ["Код"] = (node, args) => node
                        .SelectSingleNode(@"//*[contains(text(), 'Код')]/../text()[1]")
                        .InnerText.Trim(),
                ["Описание"] = (node, args) => node
                    .SelectSingleNode(@"//p[@class='textsm' and preceding-sibling::table]")
                    ?.InnerHtml ?? String.Empty,
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
                    ._SelectNodes(@"//*[@class='good_good']")
                    .Count != 0,
                findProducts: (node, args) =>node
                        ._SelectNodes(@"//*[@class='good_good']//a[contains(text(), 'Подробнее')]")
                        .Select(x => new ArgumentObject(mainUrl + x.Attributes["href"].Value))
                        .ToArray(),
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => new string('!', (int)args.Args[0]) + node
                        .SelectSingleNode(@"//img[@alt]/following-sibling::a[1]")
                        .InnerText,
                },
                singlePropertiesProduct: singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//td[@class='textsm']/a[img]")
                        .Select(x => mainUrl + x.Attributes["href"].Value)
                        .Select(x => x.Substring(29).Replace(".jpg", string.Empty))
                        .Select(x => $"http://img.simba-trade.ru/site1/{x}_c.jpg")
                        .ToArray()
                },
                encoding: Encoding.Default
                );
            var arguments = new[] { 651, 382, 385, 387, 827, 678, 680, 672, 208, 677, 682, }
            //var arguments = new[] { 827 }
                .Select(x => new ArgumentObject(url: addedUrl + x,
                                        args: new object[] {2}));
            var collection = Merger.Merge(
                collection: parser.GetProductOrCategory(arguments),
                other: IgrRuDataExtractorExample.Extract(),
                setKeyCollection: o => o.IsCategory ? o.SingleProperties["Наименование"] :o.SingleProperties["Код"],
                setKeyOtherCollection: o => o.SingleProperties["Код"]);
            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Temporary"}, isCategory: true),
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "!IgrRu"}, isCategory: true)
            }.Extend(collection);
            Import.Write(path: "igrRu.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);

        }
    }
}
