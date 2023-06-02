using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.TinderAssistent
{
    public class IncomeMessage
    {
        public IncomeMessage() { }
        public string CONTACTER { get; set; }
        public int UNREAD_COUNT { get; set; }
        public UnreadMessage[] MESSAGES { get; set; }
    }
}
