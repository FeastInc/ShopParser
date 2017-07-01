using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataParser.Examples;
using DataParser.HelperClasses;
using HtmlAgilityPack;
using System.Net;
using System.Text;

namespace DataParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Pelikan7Example.Parse();
            //AlternaExample.Parse();
        }
    }
}
