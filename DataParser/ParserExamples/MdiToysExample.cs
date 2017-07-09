using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class MdiToys
    {
        public static void Parse()
        {
            var parser = new LiquiMolyClass(
                isCategory: node => node
                    ._SelectNodes(@"//div[@class='filter-block']")
                    .Count != 0,
                findProducts: (node, args) => node
                    ._SelectNodes(@"//*[@id='products_grid']/div/div/a")
                    .Select(x => new ArgumentObject(x.Attributes["href"].Value))
                    .ToArray(),
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => node 
                        .SelectSingleNode(@"//ul[@class]/li[contains(@class, ""active"")]/a")
                        .InnerText
                }, 
                singlePropertiesProduct: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => node
                        .SelectSingleNode(@"//h1")
                        .InnerHtml,
                    [@"""Код артикула"""] = (node, args) => node
                        .SelectSingleNode(@"//*[@id='top']/div[6]/div[1]/div[2]/div[1]/span[2]")
                        .InnerText,
                    [@"Габариты"] = (node, args) => node
                        .SelectSingleNode(@"//*[@id='top']/div[6]/div[1]/div[2]/div[5]/span[2]")
                        .InnerText,
                    [@"Цена"] = (node, args) => node
                        .SelectSingleNode(@"//*[@id='top']/div[6]/div[1]/div[2]/div[6]/a/span[1]")
                        .InnerText,
                    [@"Описание"] = (node, args) => node
                        .SelectSingleNode(@"//*[@id='top']/div[6]/div[1]/div[2]/div[7]")
                        .InnerHtml +
                        string.Join("\n", node
                        ._SelectNodes(@"//*[@id='top']/div[6]/div[1]/div[2]/div[position() > 1 and position() < last() - 1]")
                        .Select(x => x.InnerHtml))
                },
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//*[@id='top']/div[6]/div[1]/div[1]/div[1]/div/img")
                        .Select(x => @"https://mdi-toys.ru" + x.Attributes["src"].Value)
                        .ToArray()
                }
                );
            var arguments = new ArgumentObject(url: "https://mdi-toys.ru/catalog/",
                args: new object[] { 0 });
            var collection = parser.GetProductOrCategory(
                parser.GetLinks(arguments, @".//*[@id='top']/div[6]/div/div/ul[1]/li[position() > 1]/a"));
            Import.Write(path: "mdi-toys.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
