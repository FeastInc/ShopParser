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
                    ["Наименование"] = row => $"{row[0]?.Value2?.ToString()?.Substring(8)} {row[1]?.Value2}",
                    ["SAE"] = row => row[2]?.Value2?.ToString(),
                    ["ACEA"] = row => row[3]?.Value2?.ToString(),
                    ["API"] = row => row[4]?.Value2?.ToString(),
                    ["Объем"] = row => $"{row[5]?.Value2} {row[6]?.Value2}",
                    ["Цена"] = row => row[7]?.Value2?.ToString(),
                },
                pluralPropertiesFunc: new Dictionary<string, Func<Range[], string[]>>(),
                startIndex: 9);
        }
    }
}
