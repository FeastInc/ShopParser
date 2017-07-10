using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DataParser.HelperClasses;

namespace DataParser.ParserExamples
{
    public static class LavaToysExample
    {
        public static void Parse()
        {
            var URL = @"http://www.lavatoys.ru";

            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//*[@class='toy__title']")
                    .InnerText,
                [@"""Код артикула"""] = (node, args) => node
                    .SelectSingleNode(@"//*[@class='toy__article']")
                    .InnerText,
                [@"""Зачеркнутая цена"""] = (node, args) => node
                    .SelectSingleNode(@"//*[@class='toy__old-price']")
                    ?.InnerText ?? string.Empty,
                ["Цена"] = (node, args) => node
                    .SelectSingleNode(@"//*[@class='toy__price-right']/strong")
                    .InnerText,
                ["Описание"] = (node, args) => String.Format("Издает звук {0} при нажатии на игрушку", node
                    .SelectSingleNode(@"//*[@class='toy__song']")
                    ?.InnerText ?? string.Empty),
                ["Размеры"] = (node, args) => "Высота: " + node
                    .SelectSingleNode(@"//*[@class='toy__params-row'][2]/td[2]")
                    .InnerText,
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
                    ._SelectNodes(@"//*[@class='js-content__back-button']")
                    .Count == 0,
                findProducts: (node, args) => node
                    ._SelectNodes(@"//a[@class='catalog-item__image-wrapper']")
                    .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value))
                    .ToArray(),
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => new string('!', (int)args.Args[0]) + node
                        .SelectSingleNode(@"//h1[@class='catalog-items__title']")
                        .InnerText
                        .Split(':')[1]
                        .Trim()
                },
                singlePropertiesProduct: singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//*[@class='js-widget']")
                        .Select(x => URL + x.Attributes["src"].Value)
                        .ToArray()
                },
                encoding: Encoding.UTF8,
                xPathPagination: (node, args) => node
                    ._SelectNodes(@"//nav[@class='paginator paginator_position_top']/a[@class='paginator__item']")
                    .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value))
                    .ToArray()
            );

            var argument = new ArgumentObject(
                url: @"http://www.lavatoys.ru/catalogue/",
                args: new object[] { 2 });

            var collection =
            parser.GetProductOrCategory(parser.GetLinks(argument,
                @"//*[@class='nav__col nav__col_width_half nav__col_type_catalog-groups']/a[@class='nav__item']",
                prefix: @"http://www.lavatoys.ru"));

            collection = new[]
            {
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "Temporary"}, isCategory: true),
                new ProductCategoryObject(
                    new Dictionary<string, string> {["Наименование"] = "!LavaToys"}, isCategory: true)
            }.Extend(collection);

            Import.Write(path: "../../../CSV/lavaToys.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
