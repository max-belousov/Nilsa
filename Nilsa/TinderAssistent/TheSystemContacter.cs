using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.TinderAssistent
{
    public class TheSystemContacter
    {
        public TheSystemContacter(string firstName, string lastName, long contID, string photoPath)
        {
            FirstName = firstName;
            LastName = lastName;
            ContID = contID;
            PhotoPath = photoPath;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long ContID { get; set; }
        public string PhotoPath { get; set; }
    }
}
