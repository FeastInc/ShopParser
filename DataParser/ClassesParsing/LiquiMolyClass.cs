using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DataParser.HelperClasses;
using HtmlAgilityPack;

namespace DataParser
{
    public class LiquiMolyClass
    {
        private Func<HtmlNode, bool> IsCategory { get; }
        private Dictionary<string, Search<string>> SinglePropertiesProduct { get; }
        private Dictionary<string, Search<string[]>> PluralPropertiesProduct { get; }
        private Dictionary<string, Search<string[]>> PluralPropertiesCategory { get; }
        private Dictionary<string, Search<string>> SinglePropertiesCategory { get; }
        private readonly Search<ArgumentObject[]> _findSubcatalogs;
        private readonly Search<ArgumentObject[]> _findProducts;
        private readonly Search<ArgumentObject[]> _xPathPagintaion;
        public bool Debug;
        private Encoding _encoding;

        public LiquiMolyClass(
            Func<HtmlNode, bool> isCategory,
            Search<ArgumentObject[]> findSubcatalogs = null,
            Search<ArgumentObject[]> findProducts = null,
            Dictionary<string, Search<string>> singlePropertiesProduct = null,
            Dictionary<string, Search<string[]>> pluralPropertiesProduct = null,
            Dictionary<string, Search<string[]>> pluralPropertiesCategory = null,
            Dictionary<string, Search<string>> singlePropertiesCategory = null,
            Search<ArgumentObject[]> xPathPagination = null,
            bool debug = true,
            Encoding encoding = null)
        {
            SinglePropertiesProduct = singlePropertiesProduct?.ToDictionary(x => x.Key, x => x.Value);
            SinglePropertiesCategory = singlePropertiesCategory?.ToDictionary(x => x.Key, x => x.Value);
            PluralPropertiesCategory = pluralPropertiesCategory?.ToDictionary(x => x.Key, x => x.Value);
            PluralPropertiesProduct = pluralPropertiesProduct?.ToDictionary(x => x.Key, x => x.Value);
            IsCategory = isCategory;
            _findProducts = findProducts ?? ((node, o) => new ArgumentObject[0]) ;
            _findSubcatalogs = findSubcatalogs ?? ((node, o) => new ArgumentObject[0]);
            _xPathPagintaion = xPathPagination ?? ((node, o) => new ArgumentObject[0]);
            Debug = debug;
            _encoding = encoding ?? Encoding.UTF8;
        }

        private IEnumerable<ArgumentObject> ParseFromAllPages(
            HtmlNode node,
            ArgumentObject args,
            Search<ArgumentObject[]> func)
        {
            foreach (var argument in func(node, args))
            {
                yield return argument;
            }
            var links = _xPathPagintaion(node, args);
            var web = new HtmlWeb {OverrideEncoding = _encoding };
            foreach (var link in links)
            {
                var htmlNode = web.Load(link.Url).DocumentNode;
                foreach (var argument in func(htmlNode, args))
                {
                    yield return argument;
                }
            }
        }

        public IEnumerable<ProductCategoryObject> GetProductOrCategory(IEnumerable<ArgumentObject> enumerableArgs)
        {
            foreach (var args in enumerableArgs)
            {
                foreach (var result in GetProductOrCategory(args))
                    yield return result;
            }
        }

        public IEnumerable<ArgumentObject> GetLinks(ArgumentObject args, 
            string xPath,
            string url = "")
        {
            var web = new HtmlWeb { OverrideEncoding = _encoding };
            var node = web.Load(args.Url).DocumentNode;
            return node._SelectNodes(xPath)
                .Select(x => new ArgumentObject(url: url + x.Attributes["href"].Value,
                                                args:args.Args));
        }

        public IEnumerable<ProductCategoryObject> GetProductOrCategory(ArgumentObject args)
        {
            var stack = new Stack<ArgumentObject>();
            stack.Push(args);
            while (stack.Count != 0)
            {
                if (Debug)
                    Console.Write(stack.Peek().ToString());
                var result = ProccessUrl(stack.Pop());
                if (result.IsCategory)
                {
                    foreach (var product in result.Products)
                        stack.Push(product);
                    foreach (var subctalog in result.Subcatalogs)
                        stack.Push(subctalog);
                }
                yield return result;
            }
        }

        private ProductCategoryObject ParseCategoryObject(HtmlNode node, ArgumentObject args)
        {
            return new ProductCategoryObject(
                    pluralProperties: PluralPropertiesCategory?.ToDictionary(x => x.Key, x => x.Value(node, args)),
                    singleProperties: SinglePropertiesCategory.ToDictionary(x => x.Key, x => x.Value(node, args)),
                    subcatalogs: ParseFromAllPages(node, args, _findSubcatalogs).ToList(),
                    products: ParseFromAllPages(node, args, _findProducts).ToList()
                );
        }

        private ProductCategoryObject ParseProductObject(HtmlNode node, ArgumentObject args)
        {
            return new ProductCategoryObject(
                    singleProperties: SinglePropertiesProduct.ToDictionary(x => x.Key, x => x.Value(node, args)),
                    pluralProperties: PluralPropertiesProduct?.ToDictionary(x => x.Key, x => x.Value(node, args))
                );
        }

        private ProductCategoryObject ProccessUrl(ArgumentObject args)
        {
            var web = new HtmlWeb {OverrideEncoding = _encoding };
            var node = web.Load(args.Url).DocumentNode;
            //File.WriteAllText("tmp.html", node.InnerHtml);
            return IsCategory(node) 
                ? ParseCategoryObject(node, args) 
                : ParseProductObject(node, args);
        }
    }
}
