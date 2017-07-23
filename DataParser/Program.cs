using System.Collections.Generic;
using System.Threading;
using DataParser.Examples;
using DataParser.ParserExamples;

namespace DataParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var pool = new List<Thread>
            {
                new Thread(DynaToneExample.Parse),
                new Thread(GeoContExample.Parse),
                new Thread(GratwestExample.Parse),
                new Thread(IgrRuExample.Parse),
                new Thread(LavaToysExample.Parse),
                new Thread(MasterasExample.Parse),
                new Thread(OksvaTmExample.Parse),
                new Thread(PlaydoradoExample.Parse),
                new Thread(PolisieToysExample.Parse),
                new Thread(RntoysExample.Parse),
                new Thread(ValdaExample.Parse),
                new Thread(VesnaKirov.Parse),
            };

            foreach (var thread in pool)
            {
                thread.Start();
            }

            foreach (var thread in pool)
            {
                thread.Join();
            }

        }
    }
}
