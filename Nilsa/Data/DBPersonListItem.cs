using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.Data
{
    class DBPersonListItem
    {
        public String UserID;
        public String UserName;
        public int SocialNetwork;

        public DBPersonListItem(int _SocialNetwork, String _UserID, String _UserName)
        {
            SocialNetwork = _SocialNetwork;
            UserID = _UserID;
            UserName = _UserName;
        }

        public override string ToString()
        {
            return this.UserName;
        }
    }
}
