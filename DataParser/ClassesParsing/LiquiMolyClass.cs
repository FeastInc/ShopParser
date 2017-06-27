using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DataParser
{
    public class LiquiMolyClass
    {
        private Func<HtmlDocument, bool> IsCategory { get; }
        private Dictionary<string, Func<HtmlDocument, ArgumentObject, string>> PropertiesProduct { get; }
        private List<string> PropetiesProductList { get; }
        private List<string> PropetiesCategoryList { get; }
        public List<string> PropertiesList { get; }
        private Dictionary<string, Func<HtmlDocument, ArgumentObject, string>> PropertiesCategory { get; }
        private Func<HtmlDocument, ArgumentObject, ArgumentObject[]> _findSubcatalogs;
        private Func<HtmlDocument, ArgumentObject, ArgumentObject[]> _findProducts;

        public LiquiMolyClass(Dictionary<string, Func<HtmlDocument, ArgumentObject, string>> propertiesProduct
            , Dictionary<string, Func<HtmlDocument, ArgumentObject, string>> propertiesCategory
            , Func<HtmlDocument, bool> isCategory
            , Func<HtmlDocument, ArgumentObject, ArgumentObject[]> findSubcatalogs
            , Func<HtmlDocument, ArgumentObject, ArgumentObject[]> findProducts)
        {
            PropertiesProduct = propertiesProduct.ToDictionary(x => x.Key, x => x.Value);
            PropetiesProductList = PropertiesProduct.Keys.ToList();
            PropertiesCategory = propertiesCategory.ToDictionary(x => x.Key, x => x.Value);
            PropetiesCategoryList = PropertiesCategory.Keys.ToList();
            PropertiesList = PropetiesCategoryList.Union(PropetiesProductList).ToList();
            IsCategory = isCategory;
            _findProducts = findProducts;
            _findSubcatalogs = findSubcatalogs;
        }

        public IEnumerable<ProductCategoryObject> GetProductOrCategory(ArgumentObject args)
        {
            var stack = new Stack<ArgumentObject>();
            stack.Push(args);
            while (stack.Count != 0)
            {
                var result = ProccessURL(stack.Pop());
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

        internal ProductCategoryObject ParseCategoryObject(HtmlDocument htmlDoc, ArgumentObject args)
        {
            return new ProductCategoryObject(
                    properties: PropertiesCategory.ToDictionary(x => x.Key, x => x.Value(htmlDoc, args)),
                    subcatalogs: _findSubcatalogs(htmlDoc, args).ToList(),
                    products: _findProducts(htmlDoc, args).ToList()
                );
        }

        internal ProductCategoryObject ParseProductObject(HtmlDocument htmlDoc, ArgumentObject args)
        {
            return new ProductCategoryObject(
                    properties: PropertiesProduct.ToDictionary(x => x.Key, x => x.Value(htmlDoc, args))
                );
        }

        internal ProductCategoryObject ProccessURL(ArgumentObject args)
        {
            var web = new HtmlWeb();
            web.OverrideEncoding = Encoding.Default;
            var htmlDoc = web.Load(args.Url);
            return IsCategory(htmlDoc) 
                ? ParseCategoryObject(htmlDoc, args) 
                : ParseProductObject(htmlDoc, args);
        }
    }
}
