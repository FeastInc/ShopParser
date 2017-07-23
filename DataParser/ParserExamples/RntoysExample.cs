using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class RntoysExample
    {
        public static void Parse()
        {
            var badString = "(RNToys)";
            var URL = @"http://www.rntoys.com";
            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//h1")
                    .InnerText.Replace(badString, String.Empty),
                [@"""Код артикула"""] = (node, args) => "RS-" + node
                    .SelectSingleNode(@".//*[@id='ctl00_MainContent_panTopRight']/div[1]/span")
                    .InnerText,
                ["Цена"] = (node, args) => Regex.Replace(node
                    .SelectSingleNode(@".//*[@id='ctl00_MainContent_panTopRight']/div[2]/span")
                    .InnerText.Replace("р.", String.Empty), @"\s+", string.Empty),
                ["Вес"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='ctl00_MainContent_panWeight']/span")
                    .InnerText,
                ["Размеры"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='ctl00_MainContent_panDimensions']/span")
                    .InnerText,
                ["Валюта"] = (node, o) => "RUB",
                [@"""Доступен для заказа"""] = (node, o) => "1",
                [@"Статус"] = (node, o) => "1",
                [@"Страна-производитель"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='ctl00_MainContent_countryElement_iconCountry']")
                    .Attributes["alt"].Value,
                ["Описание"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='ctl00_MainContent_panFeature']/span")
                    .InnerText
            };
            singlePropertiesProduct["Заголовок"] = (node, args) => singlePropertiesProduct["Наименование"](node, args);
            singlePropertiesProduct[@"""Ссылка на витрину"""] = (node, args) =>
                Humanization.GetHumanLink(singlePropertiesProduct["Наименование"](node, args)
                                          + "-" + singlePropertiesProduct[@"""Код артикула"""](node, args));

            var parser = new LiquiMolyClass(
                isCategory: node => node
                    ._SelectNodes(@"//a[contains(@class, 'rpSelected')]")
                    .Count != 0,
                findProducts: (node, args) => node
                    ._SelectNodes(@"//div[@class='mm-pl-tiles-Item']/div/div[2]/a")
                    .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value))
                    .ToArray(),
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => new string('!', (int)args.Args[0]) + node
                        .SelectSingleNode(@"//h1")
                        .InnerText,
                    ["Описание"] = (node, args) => node
                        .SelectSingleNode(@"//h1/..")
                        .InnerHtml
                },
                singlePropertiesProduct:  singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@".//*[@id='ctl00_MainContent_panImageContainer']/div/div/img")
                        .Select(x => URL + x.Attributes["src"].Value)
                        .Select(HttpUtility.UrlPathEncode)
                        .ToArray()
                }
            );

            var argument = new ArgumentObject(
                url: URL,
                args: new object[] { 2 });

            var collection =
                parser.GetProductOrCategory(parser.GetLinks(argument,
                    @".//*[@id='ctl00_productCategoriesMenu_menu']/ul/li/a",
                    URL));
            //parser.GetProductOrCategory(argument);
            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Temporary2"}, isCategory: true),
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "!Rntoys"}, isCategory: true)
            }.Extend(collection);
            Import.Write(path: @"..\..\..\CSV\rntoys.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
