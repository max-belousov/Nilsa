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
        public string cid { get; set; }
        public int unread_count { get; set; }
        public string msg_status { get; set; }
        public string type_status { get; set; }
        public string date_time { get; set; }
    }
}
