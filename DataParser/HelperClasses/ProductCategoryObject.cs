using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class ProductCategoryObject
    {
        public Dictionary<string, string> Properties { get; }
        public List<string> PropertiesList { get; }
        public List<ArgumentObject> Subcatalogs { get;}
        public List<ArgumentObject> Products { get; }
        public bool IsCategory { get; }

        public ProductCategoryObject(Dictionary<string, string> properties, bool isCategory = false)
        {
            Properties = properties.ToDictionary(x => x.Key, x => x.Value);
            PropertiesList = properties.Keys.ToList();
            IsCategory = isCategory;
        }

        public ProductCategoryObject(Dictionary<string, string> properties
            , List<ArgumentObject> subcatalogs
            , List<ArgumentObject> products
            , bool isCategory = true)
        {
            Properties = properties.ToDictionary(x => x.Key, x => x.Value);
            PropertiesList = properties.Keys.ToList();
            Subcatalogs = subcatalogs.ToList();
            Products = products.ToList();
            IsCategory = isCategory;
        }

    }
}
