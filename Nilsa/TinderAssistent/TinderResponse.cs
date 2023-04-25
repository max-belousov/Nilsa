using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.TinderAssistent
{
    internal class TinderResponse
    {
        public TinderResponse() { }
        public int Status{ get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
}
