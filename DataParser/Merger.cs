using System;
using System.Collections.Generic;
using System.Linq;
using DataParser.HelperClasses;

namespace DataParser
{
    class Merger
    {
        public static IEnumerable<ProductCategoryObject> Merge(
            IEnumerable<ProductCategoryObject> collection,
            IEnumerable<ProductCategoryObject> other,
            Func<ProductCategoryObject, string> setKeyCollection,
            Func<ProductCategoryObject, string> setKeyOtherCollection
            )
        {
            var collectionToDictionary = collection
                .Distinct(new ProductEqualityComparer(setKeyCollection))
                .ToDictionary(setKeyCollection, x => x);
            var otherToDictonary = other
                .Distinct(new ProductEqualityComparer(setKeyOtherCollection))
                .ToDictionary(setKeyOtherCollection, x => x);
                foreach (var obj in collectionToDictionary)
                {
                    if (otherToDictonary.ContainsKey(obj.Key))
                    {
                        collectionToDictionary[obj.Key].Update(otherToDictonary[obj.Key]);
                    }
                }
            foreach (var o in collectionToDictionary)
                yield return o.Value;
        }
    }
}
