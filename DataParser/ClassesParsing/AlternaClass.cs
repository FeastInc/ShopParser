using System;
using System.Collections.Generic;
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
        public Dictionary<string, Search<string>> PropertiesProduct { get; }

        public AlternaClass(string blockExp, string refProductExp
            , Dictionary<string, Search<string>> propertiesCategory
            , Dictionary<string, Search<string>> propertiesProduct)
        {
            BlockExp = blockExp;
            RefProductExp = refProductExp;
            PropertiesCategory = propertiesCategory.ToDictionary(x => x.Key, x => x.Value);
            PropertiesProduct = propertiesProduct.ToDictionary(x => x.Key, x => x.Value);
        }

        public HtmlNode GetHtmlNode(ArgumentObject args)
        {
            var web = new HtmlWeb {OverrideEncoding = Encoding.Default};
            return web.Load(args.Url).DocumentNode;
        }

        public IEnumerable<ProductCategoryObject> GetProductOrCategory(ArgumentObject args)
        {
            foreach (var block in GetHtmlNode(args).SelectNodes(BlockExp))
            {
                yield return new ProductCategoryObject(
                    PropertiesCategory.ToDictionary(x => x.Key, x => x.Value(block, args)), isCategory:true);
                foreach (var reference in block.SelectNodes(block.XPath + RefProductExp))
                {
                    var arguments = new ArgumentObject(url: reference.Attributes["href"].Value, args: args.Args);
                    var node = GetHtmlNode(arguments);
                    yield return new ProductCategoryObject(
                        PropertiesProduct.ToDictionary(x => x.Key, x => x.Value(node, arguments)));
                }
            }
        }
    }
}
