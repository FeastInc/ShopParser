using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DataParser
{
    class Parser
    {
        public IEnumerable<ProductCategoryObject> GetProductsAndCategory(string url)
        {
            var stack = new Stack<List<string>>();
            stack.Push(new List<string> {url,});
            while (stack.Count != 0)
            {
                // var result = 
                yield return new ProductCategoryObject();
            }
        }

        public ProductCategoryObject ParseCategoryObject(HtmlDocument htmlDoc)
        {
            return new ProductCategoryObject();
        }

        public ProductCategoryObject ProccessURL(string url)
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            return new ProductCategoryObject();
        }
    }
}
