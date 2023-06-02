using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nilsa.Data;

namespace Nilsa.TinderAssistent
{
	internal class TinderRequest
	{
		//private string _socialNetwork;
		private int _iSocialNetwork;
		public TinderRequest(string mode, string command, string personePhone, string tinderId, PersoneAllData persone, int socialNetwork,string cookies = null, string charName = null)
		{
			Mode = mode;
			Command = command; 
			PersonePhone = personePhone;
			TinderId = tinderId;
			CharacteristicName = charName;
			Persone = persone;
			Cookies = cookies;
			_iSocialNetwork = socialNetwork;
            //switch (socialNetwork)
            //{
            //    case 0:
            //        Network = "VK";
            //        break;
            //    case 1:
            //        Network = "Nilsa";
            //        break;
            //    case 2:
            //        Network = "FaceBook";
            //        break;
            //    case 3:
            //        Network = "Tinder";
            //        break;
            //}
        }
		public string Mode { get; set; }
		public string Command { get; set; }
        public string Network
        {
            //get
            //{
            //	return _socialNetwork;
            //}
            get
            {
                switch (_iSocialNetwork)
                {
                    case 0:
                        return "VK";
                    case 1:
                        return "Nilsa";
                    case 2:
                        return "FaceBook";
                    case 3:
                        return "Tinder";
                    default:
                        return "Unknown Network";
                }
            }
        }
        public string PersonePhone { get; set; }
		public string TinderId { get; set; }
		public string CharacteristicName { get; set; }
		public string AuthorisationCode { get; set; }
		public string Cookies { get; set; }
		public PersoneAllData Persone { get; set; }
		//public string Network { get; set; }
		
	}
}
