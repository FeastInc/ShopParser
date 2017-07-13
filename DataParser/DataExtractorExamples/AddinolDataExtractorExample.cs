using System;
using System.Collections.Generic;
using System.Linq;
using DataParser.HelperClasses;
using Microsoft.Office.Interop.Excel;

namespace DataParser.DataExtractorExamples
{
    class AddinolDataExtractorExample
    {
        public static IEnumerable<ProductCategoryObject> Extract()
        {
            return DataExtractor.Extract(
                path: @"D:\GitHub\ShopParser\DataParser\bin\Debug\Addinol.xls",
                filter: row => row.Count(s => s != null) > 1,
                pagesToExtact: new HashSet<int> {1},
                singlePropertiesFunc: new Dictionary<string, Func<Range[], string>>
                {
                    ["Наименование"] = row => $"{row[0]._Value2().Substring(8)} {row[1]._Value2()}",
                    ["SAE"] = row => row[2]._Value2(),
                    ["ACEA"] = row => row[3]._Value2(),
                    ["API"] = row => row[4]._Value2(),
                    ["Объем"] = row => $"{row[5]._Value2()} {row[6]._Value2()}",
                    ["Цена"] = row => row[7]._Value2(),
                },
                pluralPropertiesFunc: new Dictionary<string, Func<Range[], string[]>>(),
                startIndex: 9);
        }
    }
}
