using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.TinderAssistent
{
    public class TinderResponse
    {
        public TinderResponse() { }
        public int STATUS{ get; set; }
        public string MESSAGE{ get; set; }
        public string TEXT { get; set; }
        public IncomeMessage[] DATA { get; set; }
        public string ID { get; set; }
        public string URL { get; set; }
        public string PATH { get; set; }
        public string FIRST_NAME_PERSONE { get; set; }
        public string LAST_NAME_PERSONE { get; set; }
        public string FIRST_NAME_CONTACTER { get; set; }
        public string LAST_NAME_CONTACTER { get; set; }
        public string CONTACTER { get; set; }
        public IncomeMessage[] NEW_MESSAGES { get; set; }
        public UnreadMessage[] UNREAD_MESSAGES { get; set;  }
    }
}
