using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class IgrushkaPermExample
    {
        public static void Parse()
        {
            var singlePropertiesProduct = new Dictionary<string, Search<string>>
            {
                ["Наименование"] = (node, args) => node
                    .SelectSingleNode(@"//*[@id='wow']/div/h2")
                    .InnerText,
                [@"""Код артикула"""] = (node, args) => node
                    .SelectSingleNode(@"//*[@id='wow']/div/div[3]/p[1]/b")
                    .InnerText,
                [@"Описание"] = (node, args) => node
                    .SelectSingleNode(@".//*[@id='hidetext']")
                    .InnerHtml

            };
            var parser = new AlternaClass(
                url: @"http://www.igrushka.perm.ru",
                blockExp: @"//div[contains(@class,'main_menu_sect')]",
                refProductExp: @"//menu/li/a",
                propertiesCategory:new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => node
                        .InnerText
                }, 
                singlePropertiesProduct: singlePropertiesProduct,
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//div[@id='slid_box']/img")
                        .Select(x => "http://www.igrushka.perm.ru/toys/" + x.Attributes["src"].Value)
                        .ToArray()
                }
                );
            var arguments = new ArgumentObject(url: "http://www.igrushka.perm.ru/toys/",
                args: new object[] { 0 });
            var collection = parser.GetProductOrCategory(arguments);
            Import.Write(path: "IgrushkaPerm.csv",
                collection: collection.ToArray(),
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
