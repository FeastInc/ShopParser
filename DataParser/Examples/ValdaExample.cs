using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class ValdaExample
    {
        public static void Parse()
        {
            var URL = @"http://valda.ru";
            var parser = new LiquiMolyClass(
                isCategory: node => node
                    ._SelectNodes(@"//*[@id='product-list']")
                    .Count != 0,
                findProducts: (node, args) => node
                    ._SelectNodes(@"//*[@id='product-list']/ul/li/a")
                    .Select(x => new ArgumentObject(URL + x.Attributes["href"].Value))
                    .ToArray(),
                singlePropertiesProduct: new Dictionary<string, Search<string>>
                {
                    ["Цена"] = (node, args) => node
                            .SelectSingleNode(@"//div[@class='add2cart']/span[2]")
                            ?.Attributes["data-price"]?.Value ?? 
                            node.SelectSingleNode(@"//div[@class='add2cart']/span[3]")
                            .Attributes["data-price"].Value,
                    ["Наименование"] = (node, args) => node
                        .SelectSingleNode(@"//article/h1/span")
                        .InnerText,
                    ["Описание"] = (node, args) => node
                        .SelectSingleNode(@".//*[@id='product-description']/p")
                        ?.InnerHtml ?? String.Empty + 
                        node.SelectSingleNode(@".//*[@id='cart-form']/p")
                        ?.InnerHtml ?? String.Empty
                },
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//div[@id='product-gallery']/div/a")
                        .Select(x => URL + x.Attributes["href"].Value)
                        .ToArray()
                },
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => node
                        .SelectSingleNode(@".//*[@id='page-content']/h1")
                        .InnerText
                }
                );
            var argument = new ArgumentObject(
                url: URL,
                //url: @"http://oksva-tm.ru/catalog/15",
                args: new object[] { 0 });

            var collection =
                parser.GetProductOrCategory(parser.GetLinks(argument,
                    @".//*[@id='page-content']/div[1]/ul/li/a",
                    URL));
            //parser.GetProductOrCategory(argument);
            Import.Write(path: "valda.csv",
                collection: collection,
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
