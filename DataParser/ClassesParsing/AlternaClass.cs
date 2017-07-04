using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataParser.HelperClasses;
using HtmlAgilityPack;

namespace DataParser.Examples
{
    class AlternaClass
    {
        public string BlockExp;
        public string RefProductExp;
        public Dictionary<string, Search<string>> PropertiesCategory { get; }
        public Dictionary<string, Search<string>> SinglePropertiesProduct { get; }
        public Dictionary<string, Search<string[]>> PluralPropertiesProduct { get; }
        public bool Debug { get; }
        public string Url { get; }

        public AlternaClass(string blockExp, string refProductExp
            , Dictionary<string, Search<string>> propertiesCategory
            , Dictionary<string, Search<string>> singlePropertiesProduct,
            Dictionary<string, Search<string[]>> pluralPropertiesProduct = null,
            bool debug = true,
            string url = "")
        {
            BlockExp = blockExp;
            RefProductExp = refProductExp;
            PropertiesCategory = propertiesCategory.ToDictionary(x => x.Key, x => x.Value);
            PluralPropertiesProduct = pluralPropertiesProduct?.ToDictionary(x => x.Key, x => x.Value);
            SinglePropertiesProduct = singlePropertiesProduct.ToDictionary(x => x.Key, x => x.Value);
            Debug = debug;
            Url = url;
        }

        public HtmlNode GetHtmlNode(ArgumentObject args)
        {
            var web = new HtmlWeb {OverrideEncoding = Encoding.Default};
            return web.Load(args.Url).DocumentNode;
        }

        public IEnumerable<ProductCategoryObject> GetProductOrCategory(ArgumentObject args)
        {
            if (Debug)
                Console.WriteLine(args.ToString());
            foreach (var block in GetHtmlNode(args).SelectNodes(BlockExp))
            {
                yield return new ProductCategoryObject(
                    PropertiesCategory.ToDictionary(x => x.Key, x => x.Value(block, args)), isCategory:true);
                foreach (var reference in block._SelectNodes(block.XPath + RefProductExp))
                {
                    var arguments = new ArgumentObject(url: Url + reference.Attributes["href"].Value, args: args.Args);
                    if (Debug)
                        Console.WriteLine(arguments.ToString());
                    var node = GetHtmlNode(arguments);
                    //File.WriteAllText("tmp.html", node.InnerHtml);
                    yield return new ProductCategoryObject(
                        SinglePropertiesProduct.ToDictionary(x => x.Key, x => x.Value(node, arguments)),
                        PluralPropertiesProduct.ToDictionary(x => x.Key, x => x.Value(node, arguments)));
                }
            }
        }
    }
}
