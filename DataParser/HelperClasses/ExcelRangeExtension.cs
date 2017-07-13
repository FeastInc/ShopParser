using System;
using Microsoft.Office.Interop.Excel;

namespace DataParser.HelperClasses
{
    public static class ExcelRangeExtension
    {
        public static string _Value2(this Range range)
        {
            return range.Value2?.ToString();
        }

        public static string _Hyperlink(this Range range, int index)
        {
            return range.Hyperlinks.Count >= index
                ? range.Hyperlinks[index].Address
                : null;
        }
    }
}
