using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class TomikExample
    {
        public static void Parse()
        {
            var url = "http://tomik.ru/";
            var singlePropertiesProduct = new Dictionary<string, Search<string>>()
            {
                [@"Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//h1[@class]")
                    .InnerText,
                [@"Цена"] = (node, args) => node
                    .SelectSingleNode(@"//div[contains(@class, 'price')]/span")
                    .InnerText,
                [@"Деталей"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='toy-block']/div/div/div/div/div/div[3]/div[1]/div[2]")
                    .InnerText.Substring(9),
                [@"""Код артикула"""] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='toy-block']/div/div/div/div/div/div[3]/div[1]/div[3]")
                    .InnerText.Substring(9),
                [@"Габариты"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='toy-block']/div/div/div/div/div/div[3]/div[1]/div[4]")
                    .InnerText.Substring(8),
                [@"Вес"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='toy-block']/div/div/div/div/div/div[3]/div[1]/div[5]")
                    .InnerText.Substring(5),
                ["Валюта"] = (node, o) => "RUB",
                [@"""Доступен для заказа"""] = (node, o) => "1",
                [@"Статус"] = (node, o) => "1",
            };
            singlePropertiesProduct["Заголовок"] = (node, args) => singlePropertiesProduct["Наименование"](node, args);
            singlePropertiesProduct[@"""Ссылка на витрину"""] = (node, args) =>
                Humanization.GetHumanLink(singlePropertiesProduct["Наименование"](node, args));

            var parser = new LiquiMolyClass(
                isCategory: node => node
                    ._SelectNodes(@"//h1[@class=""popular-items-cell-T""]")
                    .Count == 0,
                findSubcatalogs: (node, args) => node
                    ._SelectNodes(@"//div[@class=""container-fluid""]/div[3]/div/a")
                    .Select(x => new ArgumentObject(url: url + x.Attributes["href"].Value
                        , args: new object[] { (int)args.Args[0] + 1 }))
                    .ToArray(),
                findProducts: (node, args) => node
                    ._SelectNodes(@"//section[contains(@class, ""items-category"")]/div/div/div/div/div/a")
                    .Select(x => new ArgumentObject(url: url + x.Attributes["href"].Value, args: args.Args))
                    .ToArray(),
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//img[@id]")
                        .Select(x => url + x.Attributes["src"].Value)
                        .Where(x => !x.Equals(String.Empty))
                        .Distinct()
                        .ToArray()
                },
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => new string('!', (int)args.Args[0]) + node
                        .SelectSingleNode(@"//h1")
                        .InnerText,
                },
                singlePropertiesProduct: singlePropertiesProduct
                );

            var argument = new ArgumentObject(
                //url: @"http://tomik.ru/",
                url: @"http://tomik.ru/katalog/nastolnye-igry/domino/",
                //url: @"http://tomik.ru/katalog/elochnye-igrushki/podarki/kopiya-shnurovka-elochka-naryadnaya,-6-detalej.html",
                args: new object[] { 0 });
            var links = parser
                .GetLinks(argument, @".//*[@id='sidebar']/div[2]/ul/li/a")
                .Select(x => new ArgumentObject(url: url + x.Url,
                    args: x.Args));

            var collection =
                //parser.GetProductOrCategory(links);
                parser.GetProductOrCategory(argument);
            Import.Write(path: "tomik.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
