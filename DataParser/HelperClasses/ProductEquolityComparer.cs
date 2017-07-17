using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.HelperClasses
{
    class ProductEqualityComparer : IEqualityComparer<ProductCategoryObject>
    {
        Func<ProductCategoryObject, string> KeySelector { get; }

        public ProductEqualityComparer(Func<ProductCategoryObject, string> keySelector)
        {
            KeySelector = keySelector;
        }

        public bool Equals(ProductCategoryObject x, ProductCategoryObject y)
        {
            return KeySelector(x) == KeySelector(y);
        }

        public int GetHashCode(ProductCategoryObject obj)
        {
            return KeySelector(obj).GetHashCode();
        }
    }
}
