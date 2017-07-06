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
            Func<ProductCategoryObject, string> setKeyOtherCollection,
            bool addMissingKeys = false
            )
        {
            var collectionToDictionary = collection
                .ToDictionary(setKeyCollection, x => x);
            var otherToDictonary = other
                .ToDictionary(setKeyOtherCollection, x => x);
            foreach (var obj in otherToDictonary)
            {
                if (!collectionToDictionary.ContainsKey(obj.Key) && addMissingKeys)
                {
                    collectionToDictionary[obj.Key] = obj.Value;
                }
                else if (collectionToDictionary.ContainsKey(obj.Key))
                {
                    collectionToDictionary[obj.Key].Update(obj.Value);
                }
            }

            foreach (var o in collectionToDictionary)
                yield return o.Value;
        }
    }
}
