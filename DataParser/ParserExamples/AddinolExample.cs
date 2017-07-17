using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;
using System.Text.RegularExpressions;
using DataParser.DataExtractorExamples;

namespace DataParser.ParserExamples
{
    static class AddinolExample
    {
        public static void Parse()
        {
            var URL = @"http://addinol.ru";

            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//h1")
                    .InnerText,
                ["Описание"] = (node, args) => node
                    .SelectSingleNode(@"//*[@id='art_container']/p")
                    ?.InnerHtml??string.Empty,
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
                    ._SelectNodes(@"//*[@id='art_container']")
                    .Count == 0,
                findProducts: (node, args) => node
                    ._SelectNodes(@"//h2/a")
                    .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value))
                    .ToArray(),
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) =>
                    {
                        var sub1 = node
                          .SelectSingleNode(@"//li[contains(@class, 'submenu_sel')]/a")
                          .InnerText;
                        var sub2 = node
                            .SelectSingleNode(@".//*[contains(@class, 'submenu_2_sel')]/a")
                            ?.InnerText;

                        return new string('!', (int)args.Args[0]) + (sub2 != null ? sub2 : sub1).Trim();
                    }
                },
                findSubcatalogs: (node, args) =>
                {
                    if ((int)args.Args[0] < 2)
                    {
                        return node
                            ._SelectNodes(@"//li[contains(@class, 'submenu_sel')]/ul/li/a")
                            .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value,
                            new object[] { (int)args.Args[0] + 1 }))
                            .ToArray();
                    }
                    else
                    {
                        return new ArgumentObject[0];
                    }
                    
                },
                singlePropertiesProduct: singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => {
                        var a = node
                            ._SelectNodes(@"//*[@id='right_col']/div/a")
                            .Select(x => x.Attributes["href"].Value)
                            .ToArray();
                        return a;
                    }
                },
                encoding: Encoding.UTF8
            );

            var argument = new ArgumentObject(
               url: @"http://addinol.ru/index.php?id=15804",
               args: new object[] { 1 });

            var collection =
            parser.GetProductOrCategory(parser.GetLinks(argument,
                @"//*[@class='submenu' and position() < 4]/a",
                prefix: URL));

            collection = Merger.Merge(
              collection: collection,
              other: AddinolDataExtractorExample.Extract(),
              setKeyCollection: o => o.SingleProperties["Наименование"],
              setKeyOtherCollection: o => o.SingleProperties["Наименование"]);

            collection = JoinerArticles.JoinInOrderEnumerable(collection, "Наименование",
                productFieldsForPluralProp: new[] { "Изображения" },
                productFieldsForSingleProp: new[] { "Описание" });

            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Addinol"}, isCategory: true)
            }.Extend(collection);

            Import.Write(path: "../../../CSV/addinol.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
