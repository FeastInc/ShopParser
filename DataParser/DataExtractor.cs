using System;
using System.Collections.Generic;
using System.Linq;
using DataParser.HelperClasses;
using Excel = Microsoft.Office.Interop.Excel;

namespace DataParser
{
    class DataExtractor
    {
        public static IEnumerable<ProductCategoryObject> Extract(
            string path,
            HashSet<int> pagesToExtact,
            Func<object[], bool> filter,
            Dictionary<string, Func<object[], string>> singlePropertiesFunc,
            Dictionary<string, Func<object[], string[]>> pluralPropertiesFunc,
            int startIndex = 1)
        { 
            var xlApp = new Excel.Application();
            var xlWorkBook = xlApp.Workbooks.Open(Filename: path, ReadOnly: true);
            foreach (var index in pagesToExtact)
            {
                var xlWorksheet = (Excel.Worksheet)xlWorkBook.Worksheets.Item[index];
                var range = xlWorksheet.UsedRange;
                for (int row = startIndex; row <= range.Rows.Count; row++)
                {
                    var rowArray = Enumerable
                        .Range(1, range.Columns.Count)
                        .Select(x => (range.Cells[row, x] as Excel.Range)?.Value2
                                        ?? String.Empty)
                        .ToArray();
                    if (filter(rowArray))
                    {
                        var singleProperties = new Dictionary<string, string>();
                        var pluralProperties = new Dictionary<string, string[]>();
                        foreach (var func in singlePropertiesFunc)
                        {
                            singleProperties[func.Key] = func.Value(rowArray);
                        }
                        foreach (var func in pluralPropertiesFunc)
                        {
                            pluralProperties[func.Key] = func.Value(rowArray);
                        }

                        yield return new ProductCategoryObject(singleProperties,
                            pluralProperties);
                    }
                }
            }
        }
    }
}
