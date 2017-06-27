using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class ArgumentObject
    {
        public string Url;
        public object[] Args;

        public ArgumentObject(string url, object[] args = null)
        {
            Url = url;
            Args = args?.ToArray();
        }
    }
}
