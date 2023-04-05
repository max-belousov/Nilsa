using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa
{
    internal class ResponseFromInterface
    {
        public ResponseFromInterface() { }
        public string Result { get; set; }
        public string PhotoUrl { get; set; }
        public string Time { get; set; }
        public long PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string Relation { get; set; }
        public string BirthDate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountersFriends { get; set; }
        public string Online { get; set; }
        public string LastSeen { get; set; }
    }
}
