using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.HelperClasses
{
    public static class CollectionExtension
    {
        public static IEnumerable<T> Extend<T>(this IEnumerable<T> collection, IEnumerable<T> other)
        {
            foreach (var el in collection)
                yield return el;
            foreach (var el in other)
                yield return el;
        }
    }
}
