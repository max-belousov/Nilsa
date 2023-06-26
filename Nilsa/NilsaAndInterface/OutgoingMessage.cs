using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.NilsaAndInterface
{
    internal class OutgoingMessage
    {
        public OutgoingMessage() { }
        public long ContacterId { get; set; }
        public string Message { get; set; }
        public string ContacterName { get; set; }
        public string Channel { get; set;}
    }
}
