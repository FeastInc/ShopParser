using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace DataParser
{
    class Program
    {
        static void Main(string[] args)
        {
<<<<<<< HEAD
            StihlExample.Parse();
=======
            foreach (var o in AddinolDataExtractorExample.Extract().Take(10))
            {
                Console.WriteLine(o);
            }
            //AddinolExample.Parse();
>>>>>>> 78bcde255a8b2d85489f36a43f8134a78b8a3b1a
        }
    }
}
