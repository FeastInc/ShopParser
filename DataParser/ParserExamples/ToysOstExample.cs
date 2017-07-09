using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.ParserExamples
{
    class ToysOstExample
    {
        public static void Parse()
        {
            var parser = new LiquiMolyClass(
                isCategory: node => false
                );
        }
    }
}
