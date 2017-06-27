using System;
using System.Collections.Generic;
using System.Linq;
using DataParser.Examples;
using DataParser.HelperClasses;
using HtmlAgilityPack;
using System.Net;
using System.Text;

namespace DataParser
{
    class Program
    {


        static void Main(string[] args)
        {
            AlternaExample.Parse();
            //var alterna = new AlternaClass(
            //    blockExp: @"//div[contains(@class, ""level_2"")]/div[@class=""item""]"
            //    , refProductExp: @"//div[@class=""uss_shop_name""]/a"
            //    , propertiesProduct: new Dictionary<string, Func<HtmlNode, ArgumentObject, string>>
            //    {
            //        ["Изображения"] = (node, o) => node
            //            .SelectSingleNode(@"//a[@class=""enlarge_image_inside""]")
            //            .Attributes["href"].Value,
            //        ["Наименование"] = (node, o) => node
            //            .SelectSingleNode(@"//h1")
            //            .InnerText,
            //        ["Цена"] = (node, o) => node
            //            .SelectSingleNode(@"//div[@class=""uss_shop_price""]/span")
            //            .InnerText,
            //        ["Описание"] = (node, o) => node
            //            .SelectSingleNode(@"//div[@class=""uss_shop_full_description""]")
            //            .InnerHtml,
            //        ["Габариты"] = (node, o) => node
            //            .SelectSingleNode(@"//div[@class=""uss_shop_technical_data""]/div[@class=""uss_shop_description""]")
            //            .InnerText,
            //    }, propertiesCategory: new Dictionary<string, Func<HtmlNode, ArgumentObject, string>>
            //    {
            //        ["Наименование"] = (node, o) => node
            //                                            .SelectSingleNode(@"//div[contains(@class, ""name"") and contains(@class, ""level_2"")]")
            //                                            .InnerText.Trim()
            //    });
            //var web = new HtmlWeb();
            //web.OverrideEncoding = Encoding.Default;
            //var htmlDoc = web.Load("http://xn--80aaoxlrm3f.xn--p1ai/store/13488/");
            //var a = new ArgumentObject(url: "1");
            //foreach (var block in htmlDoc.DocumentNode.SelectNodes(@"//div[contains(@class, ""level_2"")]/div[@class=""item""]"))
            //{
            //    var result = new ProductCategoryObject(
            //        alterna.PropertiesCategory.ToDictionary(x => x.Key, x => x.Value(block, a)), true);
            //    Console.WriteLine(block
            //        .XPath);
            //    //.SelectSingleNode(block.XPath+@"//div[contains(@class, ""name"") and contains(@class, ""level_2"")]")
            //    //.InnerText.Trim());
            //    //foreach (var reference in block.SelectNodes(@"//div[@class=""uss_shop_name""]/a"))
            //    //{
            //    //    var arguments = new ArgumentObject(url: reference.Attributes["href"].Value, args: args.Args);
            //    //    var node = GetHtmlNode(arguments);
            //    //    yield return new ProductCategoryObject(
            //    //        PropertiesProduct.ToDictionary(x => x.Key, x => x.Value(node, arguments)));
            //    //}
            //}
        }
    }
}
