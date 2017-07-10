using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class MirkukolivanovoExample
    {
        public static void Parse()
        {
            //var parser = new LiquiMolyClass(
            //    isCategory: node => node
            //        ._SelectNodes(@".//*[@id='main']/div[1]/div/main/div/div/ul[1]/li/span")
            //        .Count != 0,
            //    findProducts: (node, args) => node
            //        ._SelectNodes(@".//*[@id='main']/div[1]/div/main/div/ul/li/div/a")
            //        .Select(x => new ArgumentObject(x.Attributes["href"].Value))
            //        .ToArray(),
            //    singlePropertiesCategory: new Dictionary<string, Search<string>>
            //    {
            //        ["Наименование"] = (node, args) => node
            //            .SelectSingleNode(@"//h1[@class='page-title']")
            //            .InnerText
            //    },
            //    pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
            //    {
            //    }
            //    );
        }
    }
}
