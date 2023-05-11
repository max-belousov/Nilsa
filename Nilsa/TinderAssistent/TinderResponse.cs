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
        public int Status{ get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public string Text { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public IncomeMessage[] New_messages { get; set; }
        public UnreadMessage[] Unread_messages { get; set;  }
    }
}
