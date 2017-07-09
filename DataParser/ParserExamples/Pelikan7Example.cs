using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DataParser.HelperClasses;
using HtmlAgilityPack;

namespace DataParser.Examples
{
    class Pelikan7Example
    {
        public static void Parse()
        {
            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, o) => node
                    .SelectSingleNode(@"//div[@id='content']//h1")
                    .InnerText,
                ["Цена"] = (node, o) => node
                    .SelectSingleNode(@"//div[@id=""content""]//table/tr[1]/td[2]/span")
                    .InnerText.Replace("руб.", String.Empty)
                    .Replace(" ", String.Empty),
                [@"""Код артикула"""] = (node, o) => node
                    .SelectSingleNode(@"//div[@id='content']/div[2]/div/table/tr/td[2]/table/tr[3]/td[2]")
                    .InnerText,
                ["Размеры"] = (node, o) => node
                    .SelectSingleNode(@"//div[@id='content']/div[2]/div/table/tr/td[2]/table/tr[5]/td[2]")
                    .InnerText,
                ["Описание"] = (node, o) => node
                    .SelectSingleNode(@"//div[@id='description']")
                    .InnerHtml,
                ["Валюта"] = (node, o) => "RUB",
                [@"""Доступен для заказа"""] = (node, o) => "1",
                [@"Статус"] = (node, o) => "1",
            };
            singlePropertiesProduct["Заголовок"] = (node, args) => singlePropertiesProduct["Наименование"](node, args);
            singlePropertiesProduct[@"""Ссылка на витрину"""] = (node, args) => 
                Humanization.GetHumanLink(singlePropertiesProduct["Наименование"](node, args));
            singlePropertiesProduct[@"""Краткое описание"""] = (node, args) => singlePropertiesProduct["Описание"](node, args)
                                                                        .Split('.')[0];
            var parser = new LiquiMolyClass(
                isCategory: node => node.SelectNodes(@"//*[@class=""list""]")?.Any() ?? false,
                findProducts: (node, o) => node
                        ._SelectNodes(@"//a[img[contains(@id, 'image')]]")
                        .Select(x => new ArgumentObject(url: x.Attributes["href"].Value))
                        .ToArray(),
                findSubcatalogs: (node, o) => node
                    ._SelectNodes(@"//div[@class=""citem""]/a[img]")
                    .Select(x => new ArgumentObject(url: x.Attributes["href"].Value
                        , args: new object[] {(int) o.Args[0] + 1}))
                    .ToArray(),
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, o) => new string('!', (int)o.Args[0]) + node
                        .SelectSingleNode(@"//div[@id='content']//h1")
                        .InnerText
                },
                singlePropertiesProduct: singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, o) => node
                        .SelectNodes(@"//div[@id='content']//table//a[@class=""sc_menu_item""]")
                        .Select(x => x.Attributes["href"].Value)
                        .ToArray()
                },
                xPathPagination: (node, args) => node
                    ._SelectNodes(@"//div[@class=""pagination""]/div[@class=""links""]/a[position() < last() - 1]")
                    .Select(x => new ArgumentObject(
                        WebUtility.HtmlDecode(@"http://oksva-tm.ru" + x.Attributes["href"].Value)))
                    .ToArray()
                );
            var argument = new ArgumentObject(
                url: @"http://www.pelikan-7.ru/index.php?route=common/home",
                //url: @"http://www.pelikan-7.ru/index.php?route=product/category&path=38_45",
                args: new object[] {0});

            var collection =
                parser.GetProductOrCategory(parser.GetLinks(argument, @".//*[@id='category_menu']/ul/li/a"));
            //parser.GetProductOrCategory(argument);
            Import.Write(path: "pelikan7.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
