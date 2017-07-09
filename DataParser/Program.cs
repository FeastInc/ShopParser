using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DataParser.DataExtractorExamples;
using DataParser.Examples;
using DataParser.ParserExamples;
using Microsoft.Office.Interop.Excel;

namespace DataParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //foreach (var o in IgrRuDataExtractorExample.Extract().Take(20))
            //{
            //    Console.WriteLine(o.ToString());
            //}
            IgrRuExample.Parse();
        }
    }
}
