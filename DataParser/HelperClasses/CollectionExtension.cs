using System.Collections.Generic;

namespace DataParser.HelperClasses
{
    public static class CollectionExtension
    {
        public static IEnumerable<T> Extend<T>(this IEnumerable<T> collection, IEnumerable<T> other)
        {
            foreach (var el in collection)
                yield return el;
            if (other != null)
            {
                foreach (var el in other)
                    yield return el;
            }
        }
    }
}
