using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa
{
    class AlgoritmID
    {
        public int ID;
        public String Name;
        public AlgoritmID(int _ID, String _Name)
        {
            ID = _ID;
            Name = _Name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
