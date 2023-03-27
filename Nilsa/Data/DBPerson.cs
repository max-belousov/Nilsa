using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.Data
{
    public class DBPerson
    {
        public String UserID;
        public String UserName;
        public String UserLogin;
        public String UserPassword;
        public int SocialNetwork;

        public DBPerson(int _SocialNetwork, String _UserID, String _UserName, String _UserLogin, String _UserPassword)
        {
            SocialNetwork = _SocialNetwork;
            UserID = _UserID;
            UserName = _UserName;
            UserLogin = _UserLogin;
            UserPassword = _UserPassword;
        }

        public static DBPerson FromRecordString(int _SocialNetwork, String value)
        {
            String sUID = value.Substring(0, value.IndexOf("|")); // usrID
            value = value.Substring(value.IndexOf("|") + 1); // skip usrID
            String sUName = value.Substring(0, value.IndexOf("|"));
            value = value.Substring(value.IndexOf("|") + 1); // skip usrName
            String sULogin = value.Substring(0, value.IndexOf("|")); // usrLogin
            value = value.Substring(value.IndexOf("|") + 1); // skip usrLogin
            String sUPwd = value;
            return new DBPerson(_SocialNetwork, sUID, sUName, sULogin, sUPwd);
        }

        public string ToRecordString()
        {
            string str = "";
            str += this.UserID + "|";
            str += this.UserName + "|";
            str += this.UserLogin + "|";
            str += this.UserPassword;

            return str;
        }

        public override string ToString()
        {
            return this.ToRecordString();
        }
    }
}