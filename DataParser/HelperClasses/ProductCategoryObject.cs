using System.Collections.Generic;
using System.Linq;

namespace DataParser
{
    public class ProductCategoryObject
    {
        public Dictionary<string, string> SingleProperties { get; }
        public Dictionary<string, string[]> PluralProperties { get; }
        public List<ArgumentObject> Subcatalogs { get;}
        public List<ArgumentObject> Products { get; }
        public bool IsCategory { get; }

        public ProductCategoryObject(
            Dictionary<string, string> singleProperties
            , Dictionary<string, string[]> pluralProperties = null
            , bool isCategory = false)
        {
            SingleProperties = singleProperties.ToDictionary(x => x.Key, x => x.Value);
            PluralProperties = pluralProperties?.ToDictionary(x => x.Key, x => x.Value)
                ?? new Dictionary<string, string[]>();
            IsCategory = isCategory;
        }

        public ProductCategoryObject(
            Dictionary<string, string> singleProperties
            , List<ArgumentObject> subcatalogs
            , List<ArgumentObject> products
            , Dictionary<string, string[]> pluralProperties = null
            , bool isCategory = true)
        {
            SingleProperties = singleProperties.ToDictionary(x => x.Key, x => x.Value);
            PluralProperties = pluralProperties?.ToDictionary(x => x.Key, x => x.Value) 
                ?? new Dictionary<string, string[]>();
            Subcatalogs = subcatalogs.ToList();
            Products = products.ToList();
            IsCategory = isCategory;
        }

        public void Update(ProductCategoryObject other)
        {
            foreach (var property in other.SingleProperties)
            {
                SingleProperties[property.Key] = property.Value;
            }

            foreach (var property in other.PluralProperties)
            {
                PluralProperties[property.Key] = property.Value;
            }
        }

    }
}
